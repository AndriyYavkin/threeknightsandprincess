using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Game.ScenesHelper;
using System;
using Game.ScenesHelper.ObjectsHelper;
using System.Linq;
namespace Game.HelperCharacters;

/// <summary>
/// Provides helper methods for character movement, pathfinding, and interaction.
/// </summary>
public class CharacterHelper
{
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

    /// <summary>
    /// Event triggered when the character starts moving.
    /// </summary>
    public event Action OnMovementStarted;

    /// <summary>
    /// Event triggered when the character stops moving.
    /// </summary>
    public event Action OnMovementStopped;

    public bool IsMoving {get; private set;} = false; 

    private readonly CharacterHeroTemplate _character;
    private readonly Camera3D _mainCamera;
    private readonly PackedScene _pathMarkerScene;
    private readonly List<Node3D> _pathMarkers = new(); // Store path markers
    private readonly List<Vector3I> _pathPoints = new(); // Store path points
    
    private int _currentPathIndex = 0;
    private bool _isTargetSelected = false; // Track if a target is selected
    private bool _stopAfterNextPoint = false;

    public CharacterHelper(Camera3D mainCamera, CharacterHeroTemplate character, PackedScene pathMarkerScene)
    {
        _mainCamera = mainCamera ?? throw new ArgumentNullException(nameof(mainCamera));
        _character = character ?? throw new ArgumentNullException(nameof(character));
        _pathMarkerScene = pathMarkerScene ?? throw new ArgumentNullException(nameof(pathMarkerScene));
    }

    /// <summary>
    /// Handles character movement physics.
    /// </summary>
    public void HandleMovementPhysics()
    {
        if (_currentPathIndex < _pathPoints.Count)
        {
            Vector3I targetGridPosition = _pathPoints[_currentPathIndex];
            Vector3 targetPosition = GridToWorldPosition(targetGridPosition, _character.Position.Y);
            Tile targetTile = Scenes.TileMap.Map[targetGridPosition.X, targetGridPosition.Z];

            if (targetTile.ContainsObject != null && IsMoving)
            {
                StopMovement();
                InteractWithObject(targetGridPosition);
            }


            float distanceToTarget = _character.GlobalPosition.DistanceTo(targetPosition);

            if (distanceToTarget <= 0.1f)
            {
                GD.Print(distanceToTarget);
                // Clear passed path markers
                ClearPassedPathMarkers(_currentPathIndex);
                _character.GlobalPosition = targetPosition;
                _character.GridPosition = targetGridPosition;

                // Check if the target tile has an object

                if (_stopAfterNextPoint)
                {
                    StopMovement();
                    return;
                }

                _currentPathIndex++;

                if (_currentPathIndex >= _pathPoints.Count)
                {
                    _character.GlobalPosition = GridToWorldPosition(_pathPoints[^1], _character.Position.Y);
                    StopMovement();
                }
            }

            Vector3 velocity = _character.Velocity;

            if(IsMoving)
            {
                Vector3 direction = (targetPosition - _character.GlobalPosition).Normalized();
                velocity = direction * _character.Speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(_character.Velocity.X, 0, _character.Speed);
                velocity.Z = Mathf.MoveToward(_character.Velocity.Z, 0, _character.Speed);
            }

            _character.Velocity = velocity;
        }
    }

    /// <summary>
    /// Handles character movement input based on mouse position.
    /// </summary>
    /// <param name="mousePos">The mouse position.</param>
    /// <param name="space">The physics space state.</param>
    public void HandleInput(Vector2 mousePos, PhysicsDirectSpaceState3D space)
    {
        if (IsMoving) { _stopAfterNextPoint = true; return; }
        if (!TryGetGridPositionFromMouse(mousePos, space, out var targetGridPosition)) return;

        if (IsPositionValid(targetGridPosition) && Scenes.TileMap.Map[targetGridPosition.X, targetGridPosition.Z].GetPassable() && _character.GridPosition != targetGridPosition)
            PathingAndMoving(targetGridPosition);
    }

    /// <summary>
    /// Calculates and visualizes a path to the target position.
    /// </summary>
    /// <param name="targetGridPosition">The target grid position.</param>
    private void PathingAndMoving(Vector3I targetGridPosition)
    {
        var startGridPosition = WorldToGridPosition(_character.GlobalPosition);
        var path = Pathfinder3D.FindPath(startGridPosition, targetGridPosition);
        
        if (path.Count > 0)
        {
            if (!_isTargetSelected || (_isTargetSelected && _pathPoints[^1] != targetGridPosition))
            {
                ClearPathMarkers();
                VisualizePath(path);
                _isTargetSelected = true;
            }
            else
            {
                StartMovement();
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
    private void VisualizePath(List<Vector3I> path)
    {
        _pathPoints.Clear();
        _pathPoints.AddRange(path);

        for (int i = 1; i < path.Count; i++)
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
    }

    /// <summary>
    /// Interacts with an object on the target tile.
    /// </summary>
    /// <param name="targetGridPosition">The grid position of the object.</param>
    private void InteractWithObject(Vector3I targetGridPosition)
    {
        var targetTile = Scenes.TileMap.Map[targetGridPosition.X, targetGridPosition.Z];
        if (targetTile.ContainsObject is IInteractable interactable)
        {
            interactable.OnInteract(_character);
            GD.Print($"Interacted with object at {targetGridPosition}.");

            // If the object is picked up, mark the tile as passable
            if (targetTile.ContainsObject is IObjectPickable)
            {
                Pathfinder3D.UpdateTileState(targetGridPosition, Scenes.TileMap.Map[targetGridPosition.X, targetGridPosition.Z].GetPassable());
                targetTile.ContainsObject = null;
            }
        }
        else
        {
            GD.PrintErr($"Object at {targetGridPosition} is not interactable!");
        }
    }

    private bool TryGetGridPositionFromMouse(Vector2 mousePos, PhysicsDirectSpaceState3D space, out Vector3I gridPosition)
    {
        gridPosition = default;
        var from = _mainCamera.ProjectRayOrigin(mousePos);
        var to = from + _mainCamera.ProjectRayNormal(mousePos) * 100f;
        var result = space.IntersectRay(PhysicsRayQueryParameters3D.Create(from, to));

        if (result.Count == 0) return false;
        gridPosition = WorldToGridPosition((Vector3)result["position"]);
        return true;
    }

    /// <summary>
    /// Starts character movement along the calculated path.
    /// </summary>
    /// <param name="targetGridPosition">The target grid position.</param>
    private void StartMovement()
    {
        IsMoving = true;
        _isTargetSelected = false;
        _stopAfterNextPoint = false;
        GD.Print($"Path: {string.Join(" -> ", _pathPoints)}");
        _currentPathIndex = 0;
        OnMovementStarted?.Invoke();
    }

    /// <summary>
    /// Stops character movement.
    /// </summary>
    private void StopMovement()
    {
        IsMoving = false;
        _stopAfterNextPoint = false;
        _currentPathIndex = 0;
        ClearPathMarkers();
        OnMovementStopped?.Invoke();
    }

    /// <summary>
    /// Clears path markers up to the specified index.
    /// </summary>
    /// <param name="index">The index up to which markers should be cleared.</param>
    private void ClearPassedPathMarkers(int index)
    {
        _pathMarkers.Take(index).ToList().ForEach(marker => marker.Visible = false);
    }

    /// <summary>
    /// Clears all path markers.
    /// </summary>
    private void ClearPathMarkers()
    {
        _pathMarkers.ForEach(marker => marker.QueueFree());
        _pathMarkers.Clear();
    }

    // <summary>
    /// Checks if a grid position is valid.
    /// </summary>
    /// <param name="position">The grid position to check.</param>
    /// <returns>True if the position is valid, otherwise false.</returns>
    private static bool IsPositionValid(Vector3I position)
    {
        return position.X >= 0 && position.X < MapWidth &&
                position.Z >= 0 && position.Z < MapHeight;
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
}