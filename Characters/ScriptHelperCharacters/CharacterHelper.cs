using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Game.ScenesHelper;
using System;
using Game.ScenesHelper.ObjectsHelper;
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
        _mainCamera = mainCamera;
        _character = character;
        _pathMarkerScene = pathMarkerScene;
    }

    /// <summary>
    /// Handles character movement physics.
    /// </summary>
    public void HandleMovementPhysics()
    {
        GD.Print(IsMoving);
        Vector3 velocity = _character.Velocity;

        if (_currentPathIndex < _pathPoints.Count)
        {
            var targetGridPosition = _pathPoints[_currentPathIndex];
            var targetPosition = GridToWorldPosition(targetGridPosition, _character.Position.Y);

            // Check if the target tile has an object
            var targetTile = Scenes.TileMap.Map[targetGridPosition.X, targetGridPosition.Z];
            if (targetTile.ContainsObject != null)
            {
                // Interact with the object
                StopMovement();
                InteractWithObject(targetGridPosition);
            }

            if(IsMoving)
            {
                Vector3 direction = (targetPosition - _character.GlobalPosition).Normalized();
                velocity = direction * _character.Speed;

                float distanceToTarget = _character.GlobalPosition.DistanceTo(targetPosition);

                if (distanceToTarget <= 0.1f)
                {
                    GD.Print("Reached point: ", targetGridPosition);
                    GD.Print("Index: ", _currentPathIndex);

                    // Clear passed path markers
                    ClearPassedPathMarkers(_currentPathIndex);
                    _character.GlobalPosition = targetPosition;

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
                        GD.Print("Movement stopped!");
                    }
                }
            }
            else
            {
                velocity.X = Mathf.MoveToward(_character.Velocity.X, 0, _character.Speed);
                velocity.Z = Mathf.MoveToward(_character.Velocity.Z, 0, _character.Speed);
            }
        }
        _character.Velocity = velocity;
    }

    /// <summary>
    /// Handles character movement input based on mouse position.
    /// </summary>
    /// <param name="mousePos">The mouse position.</param>
    /// <param name="space">The physics space state.</param>
    public void HandleInput(Vector2 mousePos, PhysicsDirectSpaceState3D space)
    {
        if (_mainCamera == null)
        {
            GD.PrintErr("Camera is null!");
            return;
        }

        if (IsMoving)
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
            if (IsPositionValid(targetGridPosition) && Scenes.TileMap.Map[targetGridPosition.X, targetGridPosition.Z].GetPassable())
            {
                PathingAndMoving(targetGridPosition);
            }
            else
            {
                GD.Print("Cannot move to this tile!");
            }
        }
    }

    /// <summary>
    /// Picks up an item from the current tile.
    /// </summary>
    /// <param name="tile">The tile containing the item.</param>
    public void InteractWithTile(Tile tile)
    {
        if (tile == null || tile.ContainsObject == null || _character == null)
            return;

        if (_character.GridPosition != tile.PositionGrid)
            return;

        if (tile.ContainsObject is IObjectPickable item)
        {
            if (_character.Inventory == null)
            {
                GD.PrintErr("Character inventory is null!");
                return;
            }
            item.LinkedItem.PickUp(_character); // applies items effect on pick up
            _character.Inventory.AddItem(item.LinkedItem);
            GD.Print($"{_character.Name} picked up {item.LinkedItem.ItemName}.");

            tile.ContainsObject.QueueFree();
            tile.ContainsObject = null;

            Pathfinder3D.UpdateTileState(tile.PositionGrid, true);
        }
        else if(tile.ContainsObject is IInteractable interactable)
        {
            interactable.OnInteract(_character);
        }
        else
        {
            GD.PrintErr($"Tile object is not an IItem. Type: {tile.ContainsObject.GetType().Name}");
        }
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
    private void VisualizePath(List<Vector3I> path)
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
            if (targetTile.ContainsObject is IItem)
            {
                targetTile.ContainsObject = null;
            }
        }
        else
        {
            GD.PrintErr($"Object at {targetGridPosition} is not interactable!");
        }
    }

    /// <summary>
    /// Starts character movement along the calculated path.
    /// </summary>
    /// <param name="targetGridPosition">The target grid position.</param>
    private void StartMovement(Vector3I targetGridPosition)
    {
        IsMoving = true;
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
        for (int i = 0; i < index; i++)
        {
            _pathMarkers[i].Visible = false;
        }
    }

    /// <summary>
    /// Clears all path markers.
    /// </summary>
    private void ClearPathMarkers()
    {
        foreach (var marker in _pathMarkers)
        {
            marker.QueueFree();
        }
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