using Godot;

namespace GameHelperCharacters;

public static class GameHelperCamera
{
    public static Camera3D mainCamera { get; set; }
    public static CharacterBody3D Character { get; set; }
    public static float MinRotationXAxis { get; set; }
    public static float MaxRotationXAxis { get; set; }
    public static float MinRotationYAxis { get; set; }
    public static float MaxRotationYAxis { get; set; }
    public static float ZoomSpeed { get; set; }
    public static float minZoomDistance { get; set; }  // Closest zoom distance
    public static float maxZoomDistance { get; set; } // Farthest zoom distance

    /// <summary>
    /// Handles camera rotation using mouse wheel. If you want to change minimum and maximum degree of rotation, you will need to change values inside of a method
    /// <para> <b>Note:</b> Should be used in Unhandled Events </para>
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
    public static void HandleZooming(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
            {
                AdjustZoom(ZoomSpeed);
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                AdjustZoom(-ZoomSpeed);
            }
        }
    }

    /// <summary>
    /// Makes all the magic with zooming. Takes two arguments
    /// </summary>
    /// <param name="zoomAmount">How much we will zoom in, zoom out(it is better to keep it lower then 1)</param>
    private static void AdjustZoom(float zoomAmount)
    {
        // Get current camera position
        Vector3 cameraPosition = mainCamera.GlobalTransform.Origin;

        // Get forward direction (negative Z in local space)
        Vector3 forward = -mainCamera.GlobalTransform.Basis.Z;

        // Calculate new position
        Vector3 newPosition = new Vector3(cameraPosition.X, cameraPosition.Y + forward.Y * zoomAmount, cameraPosition.Z);

        // Clamp distance to prevent extreme zooming
        float distance = (newPosition - Character.Position).Length();
        if (distance < minZoomDistance || distance > maxZoomDistance)
            return; // Don't apply zoom if out of bounds

        mainCamera.GlobalTransform = new Transform3D(mainCamera.GlobalTransform.Basis, newPosition);
    }

}