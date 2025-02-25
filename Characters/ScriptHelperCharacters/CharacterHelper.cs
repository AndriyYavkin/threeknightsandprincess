using System.Collections.Generic;
using Characters;
using Godot;
using Godot.Collections;
using ScenesHelper;
using ObjectsScripts;
using System;

namespace GameHelperCharacters;

/// <summary>
/// Provides helper methods for character movement, pathfinding, and interaction.
/// </summary>
public static class CharacterHelper
{
    /// <summary>
    /// Event triggered when the character starts moving.
    /// </summary>
    public static event Action OnMovementStarted;

    /// <summary>
    /// Event triggered when the character stops moving.
    /// </summary>
    public static event Action OnMovementStopped;

    /// <summary>
    /// Event triggered when a path is calculated.
    /// </summary>
    public static event Action<List<Vector3I>> OnPathCalculated;

    /// <summary>
    /// The speed of the character.
    /// </summary>
    public static float Speed { get; set; }

    /// <summary>
    /// The width of the map in tiles.
    /// </summary>
    public static int MapWidth { get; set; }

    /// <summary>
    /// The height of the map in tiles.
    /// </summary>
    public static int MapHeight { get; set; }

    /// <summary>
    /// The size of each grid cell in world units.
    /// </summary>
    public static float GridPositionConverter { get; set;} 

    private static Camera3D _mainCamera { get; set; }
    private static CharacterTest3D _character { get; set; }
    private static PackedScene _pathMarkerScene { get; set; } // Scene for path markers
    private static List<Node3D> _pathMarkers = new(); // Store path markers
    private static List<Vector3I> _pathPoints = new(); // Store path points
    private static int _currentPathIndex = 0;
    private static bool _isTargetSelected = false; // Track if a target is selected
    private static bool _isMoving = false; 
    private static bool _stopAfterNextPoint = false;

    /// <summary>
    /// Initializes the CharacterHelper with the main camera and character.
    /// </summary>
    /// <param name="mainCamera">The main camera.</param>
    /// <param name="character">The character.</param>
    /// <param name="pathMarkerScene">The scene used for path markers.</param>
    public static void Initialize(Camera3D mainCamera, CharacterTest3D character, PackedScene pathMarkerScene)
    {
        _mainCamera = mainCamera ?? throw new ArgumentNullException(nameof(mainCamera));
        _character = character ?? throw new ArgumentNullException(nameof(character));
        _pathMarkerScene = pathMarkerScene ?? throw new ArgumentNullException(nameof(pathMarkerScene));
    }


    /// <summary>
    /// Handles character movement physics.
    /// </summary>
    /// <returns>The velocity vector for the character.</returns>
    public static Vector3 HandlePlayerMovementsPhysics()
    {
        Vector3 velocity = _character.Velocity;

        if (_isMoving && _currentPathIndex < _pathPoints.Count)
        {
            var targetGridPosition = _pathPoints[_currentPathIndex];
            var targetPosition = GridToWorldPosition(targetGridPosition, _character.Position.Y);

            Vector3 direction = (targetPosition - _character.GlobalPosition).Normalized();
            velocity = direction * Speed;

            float distanceToTarget = _character.GlobalPosition.DistanceTo(targetPosition);

            if (distanceToTarget <= 0.1f)
            {
                GD.Print("Reached point: ", targetGridPosition);
                GD.Print("Index: ", _currentPathIndex);
                ClearPassedPathMarkers(_currentPathIndex);
                _character.GlobalPosition = targetPosition;

                if (_stopAfterNextPoint)
                {
                    StopMovement();
                }
                else
                {
                    _currentPathIndex++;

                    if (_currentPathIndex >= _pathPoints.Count)
                    {
                        _character.GlobalPosition = GridToWorldPosition(_pathPoints[^1], _character.Position.Y);
                        StopMovement();
                        GD.Print("Movement stopped!");
                    }
                }
            }
        }
        else
        {
            velocity.X = Mathf.MoveToward(_character.Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(_character.Velocity.Z, 0, Speed);
        }

        return velocity;
    }

    /// <summary>
    /// Picks up an item from the current tile.
    /// </summary>
    /// <param name="tile">The tile containing the item.</param>
    public static void PickUpItem(Tile tile)
    {
        if (tile == null || tile.Object == null || _character == null)
            return;

        if (_character.GridPosition != tile.PositionGrid)
            return;

        if (tile.Object is IItem item)
        {
            if (_character.Inventory == null)
            {
                GD.PrintErr("Character inventory is null!");
                return;
            }

            _character.Inventory.AddItem(item);
            GD.Print($"{_character.Name} picked up {item.ItemName}.");

            tile.Object.QueueFree();
            tile.Object = null;

            Pathfinder3D.UpdateTileState(tile.PositionGrid, true);
        }
        else
        {
            GD.PrintErr($"Tile object is not an IItem. Type: {tile.Object.GetType().Name}");
        }
    }

    /// <summary>
    /// Handles character movement input based on mouse position.
    /// </summary>
    /// <param name="mousePos">The mouse position.</param>
    /// <param name="space">The physics space state.</param>
    public static void HandleCharacterMovements(Vector2 mousePos, PhysicsDirectSpaceState3D space)
    {
        if (_mainCamera == null)
        {
            GD.PrintErr("Camera is null!");
            return;
        }

        if (_isMoving)
        {
            _stopAfterNextPoint = true;
            GD.Print("Movement interrupted!");
            return;
        }

        // Create a ray from the camera
        Vector3 from = _mainCamera.ProjectRayOrigin(mousePos);
        Vector3 to = from + _mainCamera.ProjectRayNormal(mousePos) * 100f; // Ray length

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to);
        Dictionary result = space.IntersectRay(query);

        if (result.Count > 0)
        {
            // Snap the target position to the grid
            var hitPosition = (Vector3)result["position"];
            var targetGridPosition = WorldToGridPosition(hitPosition);

            // Check if the target position is valid and passable
            if (IsPositionValid(targetGridPosition) && Scenes.TileMap.Map[targetGridPosition.X, targetGridPosition.Z].IsPassable)
            {
                // Calculate path using the pathfinder
                PathingAndMoving(targetGridPosition);
            }
            else
            {
                GD.Print("Cannot move to this tile!");
            }
        }
    }

    /// <summary>
    /// Calculates and visualizes a path to the target position.
    /// </summary>
    /// <param name="targetGridPosition">The target grid position.</param>
    private static void PathingAndMoving(Vector3I targetGridPosition)
    {
        var startGridPosition = WorldToGridPosition(_character.GlobalPosition);
        var path = Pathfinder3D.FindPath(startGridPosition, targetGridPosition);

        if (path.Count > 0 && path.Count <= _character.RemainingMovement)
        {
            if (!_isTargetSelected || (_isTargetSelected && _pathPoints[^1] != targetGridPosition))
            {
                ClearPathMarkers();
                VisualizePath(path);
                _isTargetSelected = true;
            }
            else
            {
                StartMovement(targetGridPosition);
            }
        }
        else
        {
            GD.Print("Not enough movement points or target is unreachable!");
        }
    }

    /// <summary>
    /// Visualizes the calculated path using markers.
    /// </summary>
    /// <param name="path">The path to visualize.</param>
    private static void VisualizePath(List<Vector3I> path)
    {
        _pathPoints.Clear();
        _pathPoints.AddRange(path);

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 worldPosition = GridToWorldPosition(path[i], 0);
            var marker = _pathMarkerScene.Instantiate<Node3D>();
            _character.GetTree().CurrentScene.AddChild(marker);
            marker.GlobalPosition = worldPosition;

            if (i < path.Count - 1)
            {
                Vector3 nextPosition = GridToWorldPosition(path[i + 1], 0);
                Vector3 direction = (nextPosition - worldPosition).Normalized();
                marker.LookAt(worldPosition + direction, Vector3.Up);
            }

            _pathMarkers.Add(marker);
        }

        OnPathCalculated?.Invoke(path);
    }

    /// <summary>
    /// Starts character movement along the calculated path.
    /// </summary>
    /// <param name="targetGridPosition">The target grid position.</param>
    private static void StartMovement(Vector3I targetGridPosition)
    {
        _isMoving = true;
        _character.GridPosition = targetGridPosition;
        _isTargetSelected = false;
        _stopAfterNextPoint = false;
        GD.Print($"Path: {string.Join(" -> ", _pathPoints)}");
        _currentPathIndex = 0;
        OnMovementStarted?.Invoke();
    }

    /// <summary>
    /// Stops character movement.
    /// </summary>
    private static void StopMovement()
    {
        _isMoving = false;
        _stopAfterNextPoint = false;
        _currentPathIndex = 0;
        ClearPathMarkers();
        OnMovementStopped?.Invoke();
    }

    /// <summary>
    /// Converts a grid position to a world position.
    /// </summary>
    /// <param name="gridPosition">The grid position.</param>
    /// <param name="yPosition">The Y position in the world.</param>
    /// <returns>The world position.</returns>
    private static Vector3 GridToWorldPosition(Vector3I gridPosition, float yPosition)
    {
        return new Vector3(
            gridPosition.X * GridPositionConverter,
            yPosition,
            gridPosition.Z * GridPositionConverter
        );
    }

    /// <summary>
    /// Converts a world position to a grid position.
    /// </summary>
    /// <param name="worldPosition">The world position.</param>
    /// <returns>The grid position.</returns>
    private static Vector3I WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector3I(
            Mathf.RoundToInt(worldPosition.X / GridPositionConverter),
            0,
            Mathf.RoundToInt(worldPosition.Z / GridPositionConverter)
        );
    }

    /// <summary>
    /// Clears path markers up to the specified index.
    /// </summary>
    /// <param name="index">The index up to which markers should be cleared.</param>
    private static void ClearPassedPathMarkers(int index)
    {
        for (int i = 0; i < index; i++)
        {
            _pathMarkers[i].Visible = false;
        }
    }

    /// <summary>
    /// Clears all path markers.
    /// </summary>
    private static void ClearPathMarkers()
    {
        foreach (var marker in _pathMarkers)
        {
            marker.QueueFree();
        }
        _pathMarkers.Clear();
    }
    
    /// <summary>
    /// Checks if a grid position is valid.
    /// </summary>
    /// <param name="position">The grid position to check.</param>
    /// <returns>True if the position is valid, otherwise false.</returns>
    private static bool IsPositionValid(Vector3I position)
    {
        return position.X >= 0 && position.X < MapWidth &&
                position.Z >= 0 && position.Z < MapHeight;
    }
}
