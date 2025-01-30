using System;
using Godot;

namespace GameHelperCharacters
{
    public static class CharacterHelper
    {
        /// <summary>
        /// Wee need it to be set from the character's class to the mainCamera. Otherwise, in the best scenario, most features with camera won't work properly
        /// </summary>
        public static Camera3D mainCamera { get; set; }

        ///<summary>
        ///<para>Takes 3 parameters: Camera3D, mouse position(viewport) and speed.</para>
        ///<para>Returns normalized Vector3 velocity.</para>
        ///<para> <b>Note:</b> Should be used only with CharacterBody3D and player controlled characters. </para>
        ///<para> <b>Note 2: Moving camera away from the character will break movements</b></para>
        ///</summary>
        public static Vector3 HandlePlayerMovementToMouse(Vector2 mouseViewport, float Speed)
        {
            Vector3 from = mainCamera.ProjectRayOrigin(mouseViewport);
            Vector3 to = from + mainCamera.ProjectRayNormal(mouseViewport);

            Vector3 velocity = Vector3.Zero;
            
            velocity.X = to.X - from.X;
            velocity.Z = to.Z - from.Z;

            return velocity.Normalized() * Speed;
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
                newYRotation = Mathf.Clamp(newYRotation, Mathf.DegToRad(-60), Mathf.DegToRad(60)); // The best option is min and max

                // Rotate X (up/down) but clamp the angle
                float newXRotation = mainCamera.Rotation.X - ev.Relative.Y * 0.005f; // Subtract to invert axis
                newXRotation = Mathf.Clamp(newXRotation, Mathf.DegToRad(-90), Mathf.DegToRad(-60)); // The best option is min -90 and max -60
                    
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

        ///<summary>
        ///<para>Takes 4 parameters: global mouse position, position of character, speed, limit of pixels where we need acceleration.</para>
        ///<para>Returns normalized Vector2 velocity.</para>
        ///<para> <b>Note:</b> Should be used only with CharacterBody2D and player controlled characters. </para>
        ///<para> <b>Note 2:</b> It is not implemented yet! Using this method will result in NotImplementedException</para>
        ///</summary>
        public static Vector2 HandlePlayerMovementToMouseWithAcceleration(Vector2 MousePosition, Vector2 CharacterPosition, float Speed)
        {
            throw new NotImplementedException();
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
    }
}