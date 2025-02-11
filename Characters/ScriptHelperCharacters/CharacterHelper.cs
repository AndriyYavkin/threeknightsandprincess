using System;
using System.ComponentModel;
using Characters;
using Godot;
using Godot.Collections;

namespace GameHelperCharacters
{
    public class CharacterHelper
    {
        /// <summary>
        /// Wee need it to be set from the character's class to the mainCamera. Otherwise, in the best scenario, most features with camera won't work properly
        /// </summary>

        public static Camera3D mainCamera { get; set; }
        public static float MinRotationXAxis { get; set; }
        public static float MaxRotationXAxis { get; set; }
        public static float MinRotationYAxis { get; set; }
        public static float MaxRotationYAxis { get; set; }
        public static float Speed { get; set; }
        public static CharacterTest3D Character { get; set; }

        private static Vector3 _targetPosition;
        private static bool _isMoving = false; 

        ///<summary>
        ///<para>Takes 1 parameters: CharacterBody3d</para>
        ///<para>Returns Vector3 velocity.</para>
        ///<para> <b>Note:</b>  </para>
        ///</summary>
        public static Vector3 HandlePlayerMovementsPhysics()
        {
            Vector3 velocity = Character.Velocity;

            // Handle movement toward target position
            if (_isMoving)
            {
                Vector3 direction = new Vector3(
                _targetPosition.X - Character.GlobalPosition.X,
                0, // Ignore Y-axis
                _targetPosition.Z - Character.GlobalPosition.Z
                ).Normalized();

                velocity = direction * Speed;
                float distanceToTarget = Character.GlobalPosition.DistanceTo(_targetPosition);

                // If the character is close to the target, move directly to it
                if (distanceToTarget <= 1f)
                {
                    velocity = Vector3.Zero;
                    Character.GlobalPosition = new Vector3(_targetPosition.X, Character.GlobalPosition.Y, _targetPosition.Z); // Snap to the exact target position
                    _isMoving = false;
                    GD.Print("Movement stoped!");
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
        
        /// <summary>
        /// Handles camera rotation using mouse wheel. If you want to change minimum and maximum degree of rotation, you will need to change values inside of a method
        /// </summary>
        /// <param name="event">Unhandled events mostly. Not tested if it will work with other type of events</param>
        public static void HandlePlayerCameraRotation(InputEvent @event)
        {
            if(Input.IsMouseButtonPressed(MouseButton.Middle) && @event is InputEventMouseMotion ev)
            {
                // Rotate Y (left/right)
                float newYRotation = mainCamera.Rotation.Y - ev.Relative.X * 0.005f; // Subtract ot invert axis
                newYRotation = Mathf.Clamp(newYRotation, Mathf.DegToRad(MinRotationYAxis), Mathf.DegToRad(MaxRotationYAxis)); // The best option is min and max

                // Rotate X (up/down) but clamp the angle
                float newXRotation = mainCamera.Rotation.X - ev.Relative.Y * 0.005f; // Subtract to invert axis
                newXRotation = Mathf.Clamp(newXRotation, Mathf.DegToRad(MinRotationXAxis), Mathf.DegToRad(MaxRotationXAxis)); // The best option is min -90 and max -60
                    
                mainCamera.Rotation = new Vector3(newXRotation, newYRotation, mainCamera.Rotation.Z);
            }
        }

        /// <summary>
        /// If used, user will be able to zoom in and zoom out using his mouse wheel. Takes 3 parameters
        /// </summary>
        /// <param name="event">Unhandled events mostly. Not tested if it will work with other type of events</param>
        /// <param name="zoomSpeed">How fast we will zoom in and zoom out. Better to keep it less then 1f</param>
        /// <param name="CharacterPosition">Position of a character whose camera this is</param>
        public static void HandleZooming(InputEvent @event, float zoomSpeed, Vector3 CharacterPosition)
        {
            if (@event is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
                {
                    AdjustZoom(zoomSpeed, CharacterPosition);
                }
                else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
                {
                    AdjustZoom(-zoomSpeed, CharacterPosition);
                }
            }
        }

        public static void HandleCharacterMovements(Vector2 mousePos, PhysicsDirectSpaceState3D space)
        {
            if (mainCamera == null)
            {
                GD.PrintErr("Camera is null!");
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
                    Mathf.RoundToInt(hitPosition.X / 2), // Adjust scaling as needed
                    0,
                    Mathf.RoundToInt(hitPosition.Z / 2)
                );
                GD.Print(targetGridPosition);

                // Check if the target position is valid and passable
                if (IsPositionValid(targetGridPosition) && Scenes.GlobalMap.map[targetGridPosition.X, 0, targetGridPosition.Z].IsPassable)
                {
                    // Calculate path using the pathfinder
                    var path = Pathfinder3D.FindPath(new Vector3I(
                        Mathf.RoundToInt(Character.GlobalPosition.X / 2),
                        0,
                        Mathf.RoundToInt(Character.GlobalPosition.Z / 2)
                    ), targetGridPosition);

                    if (path.Count > 0 && path.Count <= Character.RemainingMovement)
                    {
                        _targetPosition = new Vector3(targetGridPosition.X * 2, 0, targetGridPosition.Z * 2); // Convert back to world coordinates
                        _isMoving = true;
                        Character.GridPosition = targetGridPosition;
                    }
                    else
                    {
                        GD.Print("Not enough movement points or target is unreachable!");
                    }
                }
                else
                {
                    GD.Print("Cannot move to this tile!");
                }
            }
        }

        /// <summary>
        /// Makes all the magic with zooming. Takes two arguments
        /// </summary>
        /// <param name="zoomAmount">How much we will zoom in, zoom out(it is better to keep it lower then 1)</param>
        /// <param name="targetPosition">Position of a character, who's camera this is</param>
        private static void AdjustZoom(float zoomAmount, Vector3 targetPosition)
        {
            float minZoomDistance = 2f;  // Closest zoom distance
            float maxZoomDistance = 7f; // Farthest zoom distance

            // Get current camera position
            Vector3 cameraPosition = mainCamera.GlobalTransform.Origin;

            // Get forward direction (negative Z in local space)
            Vector3 forward = -mainCamera.GlobalTransform.Basis.Z;

            // Calculate new position
            Vector3 newPosition = new Vector3(cameraPosition.X, cameraPosition.Y + forward.Y * zoomAmount, cameraPosition.Z);

            // Clamp distance to prevent extreme zooming
            float distance = (newPosition - targetPosition).Length();
            if (distance < minZoomDistance || distance > maxZoomDistance)
                return; // Don't apply zoom if out of bounds

            mainCamera.GlobalTransform = new Transform3D(mainCamera.GlobalTransform.Basis, newPosition);
        }

        private static bool IsPositionValid(Vector3I position)
        {
            return position.X >= -Scenes.GlobalMap.MapWidth/2 && position.X < Scenes.GlobalMap.MapWidth/2 &&
                   position.Z >= -Scenes.GlobalMap.MapHeight/2 && position.Z < Scenes.GlobalMap.MapHeight/2;
        }
    }
}