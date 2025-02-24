using System;
using System.Collections.Generic;
using Characters;
using Godot;
using Godot.Collections;
using ScenesHelper;

namespace GameHelperCharacters
{
    /// <summary>
    /// Class to help with basics characters features. moving character, path finding. Only player's characters should use this class!
    /// </summary>
    public static class CharacterHelper
    {
        public static Camera3D mainCamera { get; set; }
        public static CharacterTest3D Character { get; set; }
        public static PackedScene pathMarkerScene { get; set; } // Scene for path markers
        public static float Speed { get; set; }
        public static int MapWidth { get; set; }
        public static int MapHeight { get; set; }
        public static float GridPositionConverter { get; set;} 

        private static List<Node3D> _pathMarkers = new(); // Store path markers
        private static List<Vector3I> _pathPoints = new(); // Store path points
        private static int _currentPathIndex = 0;
        private static bool _isTargetSelected = false; // Track if a target is selected
        private static bool _isMoving = false; 
        private static bool _stopAfterNextPoint = false;

        ///<summary>
        ///<para>Returns Vector3 velocity. Works with movements</para>
        ///<para> <b>Note:</b> Should be usen in _PhysicsProcess</para>
        ///</summary>
        public static Vector3 HandlePlayerMovementsPhysics()
        {
            Vector3 velocity = Character.Velocity;
            // Handle movement toward target position

            // If there is only 1 pathpoint, we should set our path index to 0 so our next if statement will work
            if(_pathPoints.Count == 1)
            {
                _currentPathIndex = 0;
            }
            
            if (_isMoving && _currentPathIndex < _pathPoints.Count)
            {
                var targetGridPosition = _pathPoints[_currentPathIndex];
                var targetPosition = new Vector3(
                    targetGridPosition.X * GridPositionConverter,
                    Character.Position.Y, // Keep the character's Y position
                    targetGridPosition.Z * GridPositionConverter
                );

                Vector3 direction = new Vector3(
                    targetPosition.X - Character.GlobalPosition.X,
                    0, // Ignore Y-axis
                    targetPosition.Z - Character.GlobalPosition.Z
                ).Normalized();

                velocity = direction * Speed;
                float distanceToTarget = Character.GlobalPosition.DistanceTo(targetPosition);

                // If the character is close to the target, move directly to it
                if (distanceToTarget <= 0.1f)
                {
                    GD.Print("Reached point: ", targetGridPosition);
                    GD.Print("index:", _currentPathIndex);
                    ClearPassedPathMarkers(_currentPathIndex);
                    // Snap to the exact target position
                    Character.GlobalPosition = targetPosition;

                    if(_stopAfterNextPoint)
                    {   
                        velocity = Vector3.Zero;
                        _isMoving = false;
                        _stopAfterNextPoint = false;
                        _currentPathIndex = 0;
                        ClearPathMarkers();
                    }
                    else
                    {
                        // Move to the next point in the path
                        _currentPathIndex++;
                        // If the character has reached the end of the path, stop moving
                        if (_currentPathIndex >= _pathPoints.Count)
                        {
                            // Ensure the character is exactly at the center of the last tile
                            Character.GlobalPosition = new Vector3(
                                _pathPoints[_pathPoints.Count - 1].X * GridPositionConverter,
                                Character.Position.Y,
                                _pathPoints[_pathPoints.Count - 1].Z * GridPositionConverter
                            );

                            // Reset all the movement
                            velocity = Vector3.Zero;
                            _isMoving = false;
                            _stopAfterNextPoint = false;
                            _currentPathIndex = 0;
                            ClearPathMarkers();
                            GD.Print("Movement stopped!");
                        }
                    }

                }
            }
            else
            {
                // Stop movement if no target
                velocity.X = Mathf.MoveToward(Character.Velocity.X, 0, Speed);
                velocity.Z = Mathf.MoveToward(Character.Velocity.Z, 0, Speed);
            }
            
            return velocity;
        }

        ///<summary>
        ///<para>Finds what position player clicked, translating position to tiles</para>
        ///<para> <b>Note:</b> Should be usen in _UnhandledEvents</para>
        ///</summary>
        public static void HandleCharacterMovements(Vector2 mousePos, PhysicsDirectSpaceState3D space)
        {
            if (mainCamera == null)
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
            Vector3 from = mainCamera.ProjectRayOrigin(mousePos);
            Vector3 to = from + mainCamera.ProjectRayNormal(mousePos) * 100f; // Ray length

            PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to);
            Dictionary result = space.IntersectRay(query);

            if (result.Count > 0)
            {
                // Snap the target position to the grid
                var hitPosition = (Vector3)result["position"];
                var targetGridPosition = new Vector3I(
                    Mathf.RoundToInt(hitPosition.X / GridPositionConverter), // Adjust scaling as needed
                    0,
                    Mathf.RoundToInt(hitPosition.Z / GridPositionConverter)
                );

                // Check if the target position is valid and passable
                if (IsPositionValid(targetGridPosition) && Scenes.TileMap.Map[targetGridPosition.X, targetGridPosition.Z].IsPassable)
                {
                    // Calculate path using the pathfinder
                    PathingAndMovingRf(targetGridPosition);
                }
                else
                {
                    GD.Print("Cannot move to this tile!");
                }
            }
        }

        public static void PickUpItem(Tile tile)
        {
            if (tile == null || tile.Object == null)
                return;

            // Ensure the character is standing exactly on the tile before picking up
            if (Character.GridPosition != tile.PositionGrid)
                return; // Ignore items from tiles we are not actually standing on

            if (tile.Object is IItem item)
            {
                if (Character.Inventory == null)
                {
                    GD.PrintErr("Character inventory is null!");
                    return;
                }

                Character.Inventory.AddItem(item);
                GD.Print($"{Character.Name} picked up {item.Name}.");

                // Remove the object from the world
                tile.Object.QueueFree();
                tile.Object = null;

                // Update pathfinding to mark tile as passable
                Pathfinder3D.UpdateTileState(tile.PositionGrid, true);
            }
            else
            {
                GD.PrintErr($"Tile object is not an IItem. Type: {tile.Object.GetType().Name}");
            }
        }

        /// <summary>
        /// Method made in order to maintain readability of HandleCharacterMovements
        /// </summary>
        private static void PathingAndMovingRf(Vector3I targetGridPosition)
        {
            var path = Pathfinder3D.FindPath(new Vector3I(
                Mathf.RoundToInt(Character.GlobalPosition.X / GridPositionConverter),
                0,
                Mathf.RoundToInt(Character.GlobalPosition.Z / GridPositionConverter)
            ), targetGridPosition);

            if (path.Count > 0 && path.Count <= Character.RemainingMovement)
            {
                if (!_isTargetSelected || (_isTargetSelected && _pathPoints[^1] != targetGridPosition))
                {
                    // First click: Select the target and visualize the path
                    ClearPathMarkers();
                    VisualizePath(path);
                    _isTargetSelected = true;
                }
                else
                {
                    _isMoving = true;
                    Character.GridPosition = targetGridPosition;
                    _isTargetSelected = false; // Reset target selection
                    _stopAfterNextPoint = false;
                    GD.Print($"Path: {string.Join(" -> ", _pathPoints)}");
                    _currentPathIndex = 0;
                }
            }
            else
            {
                GD.Print("Not enough movement points or target is unreachable!");
            }
        }

        private static Tile GetCharacterCurrentTile()
        {
            if (Character.GridPosition.X >= 0 && Character.GridPosition.X < Scenes.TileMap.Map.GetLength(0) &&
                Character.GridPosition.Z >= 0 && Character.GridPosition.Z < Scenes.TileMap.Map.GetLength(1))
            {
                return Scenes.TileMap.Map[Character.GridPosition.X, Character.GridPosition.Z];
            }

            return null;
        }

        /// <summary>
        /// Visualize path of the character to the point player chose.
        /// </summary>
        /// <param name="path">Need it to be Pathfinder3D.FindPath(...)</param>
        private static void VisualizePath(List<Vector3I> path)
        {
            if (pathMarkerScene == null)
            {
                GD.PrintErr("Path marker scene is not set!");
                return;
            }

            // Store the path points
            _pathPoints = path;

            for (int i = 0; i < path.Count; i++)
            {
                // Convert grid position to world position
                Vector3 worldPosition = new Vector3(
                    path[i].X * GridPositionConverter,
                    0, // Y position for markers
                    path[i].Z * GridPositionConverter
                );

                // Spawn a path marker
                var marker = pathMarkerScene.Instantiate<Node3D>();
                Character.GetTree().CurrentScene.AddChild(marker);
                marker.GlobalPosition = worldPosition;

                // Rotate the arrow to face the next point in the path
                if (i < path.Count - 1)
                {
                    Vector3 nextPosition = new Vector3(
                        path[i + 1].X * GridPositionConverter,
                        0,
                        path[i + 1].Z * GridPositionConverter
                    );
                    Vector3 direction = (nextPosition - worldPosition).Normalized();
                    marker.LookAt(worldPosition + direction, Vector3.Up);
                }

                // Store the marker for later removal
                _pathMarkers.Add(marker);
            }
        }

        /// <summary>
        /// Clears path markers as character moves 
        /// </summary>
        private static void ClearPassedPathMarkers(int index)
        {
            for(int i = 0; i < index; i++)
            {
                _pathMarkers[i].Visible = false;
            }
        }

        /// <summary>
        /// Clear all path markers character has
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
        ///  Checks if the position is inside of the boundaries of the map
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private static bool IsPositionValid(Vector3I position)
        {
            return position.X >= 0 && position.X < MapWidth &&
                   position.Z >= 0 && position.Z < MapHeight;
        }
    }
}