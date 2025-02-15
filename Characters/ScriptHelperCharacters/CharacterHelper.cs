using System;
using System.Collections.Generic;
using System.ComponentModel;
using Characters;
using Godot;
using Godot.Collections;

namespace GameHelperCharacters
{
    /// <summary>
    /// Class to help with basics characters features. moving character, path finding
    /// </summary>
    public class CharacterHelper
    {
        public static Camera3D mainCamera { get; set; }
        public static CharacterTest3D Character { get; set; }
        public static float Speed { get; set; }
        public static float GridPositionConverter { get; set;} 
        public static PackedScene pathMarkerScene { get; set; } // Scene for path markers

        private static List<Node3D> _pathMarkers = new(); // Store path markers
        private static List<Vector3I> _pathPoints = new(); // Store path points
        //private static Vector3I _selectedTargetGridPosition; // Store the selected target position
        private static int _currentPathIndex = 0;
        private static bool _isTargetSelected = false; // Track if a target is selected
        private static bool _isMoving = false; 

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
                    // Snap to the exact target position
                    Character.GlobalPosition = targetPosition;

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

                        velocity = Vector3.Zero;
                        _isMoving = false;
                        ClearPathMarkers(); // Clear all path markers
                        GD.Print("Movement stopped!");
                    }
                }
                else
                {
                    ClearPassedPathMarkers();
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
                ClearPathMarkers();
                _isMoving = false;
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
                if (IsPositionValid(targetGridPosition) && Scenes.GlobalMap.map[targetGridPosition.X, 0, targetGridPosition.Z].IsPassable)
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
                    VisualizePath(path);
                    _isTargetSelected = true;
                }
                else
                {
                    _isMoving = true;
                    Character.GridPosition = targetGridPosition;
                    _isTargetSelected = false; // Reset target selection
                    GD.Print($"Path: {string.Join(" -> ", _pathPoints)}");
                    _currentPathIndex = 0;
                }
            }
            else
            {
                GD.Print("Not enough movement points or target is unreachable!");
            }
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

            // Clear existing path markers
            ClearPathMarkers();

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
        private static void ClearPassedPathMarkers()
        {
            if (_pathPoints.Count == 0 || _pathMarkers.Count == 0)
                return;

            // Get the character's current grid position
            var characterGridPosition = new Vector3I(
                Mathf.RoundToInt(Character.GlobalPosition.X / GridPositionConverter),
                0,
                Mathf.RoundToInt(Character.GlobalPosition.Z / GridPositionConverter)
            );

            // Find the index of the current path point
            int index = _pathPoints.FindIndex(p => p == characterGridPosition);

            if (index >= 0)
            {
                // Remove all markers before the current position
                for (int i = 0; i < index; i++)
                {
                    _pathMarkers[i].QueueFree(); // Remove the marker from the scene
                }

                // Remove the cleared markers from the lists
                _pathMarkers.RemoveRange(0, index);
                _pathPoints.RemoveRange(0, index);
            }
        }

        /// <summary>
        /// Clear all path markers character has
        /// </summary>
        private static void ClearPathMarkers()
        {
            foreach (var marker in _pathMarkers)
            {
                marker.QueueFree(); // Remove the marker from the scene
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
            return position.X >= 0 && position.X < Scenes.GlobalMap.MapWidth &&
                   position.Z >= 0 && position.Z < Scenes.GlobalMap.MapHeight;
        }
    }
}