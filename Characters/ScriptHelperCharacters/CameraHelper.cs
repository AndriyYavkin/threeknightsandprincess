using System;
using Godot;

namespace GameHelperCharacters;

/// <summary>
/// Provides functionality for handling camera rotation and zooming in a 3D game.
/// </summary>
public static class GameHelperCamera
{
    /// <summary>
    /// The main camera used for player perspective.
    /// </summary>
    private static Camera3D _mainCamera;

    /// <summary>
    /// The character associated with the camera.
    /// </summary>
    private static CharacterBody3D _character;

    /// <summary>
    /// The minimum rotation angle around the X-axis (in degrees).
    /// </summary>
    public static float MinRotationXAxis { get; set; }

    /// <summary>
    /// The maximum rotation angle around the X-axis (in degrees).
    /// </summary>
    public static float MaxRotationXAxis { get; set; }

    /// <summary>
    /// The minimum rotation angle around the Y-axis (in degrees).
    /// </summary>
    public static float MinRotationYAxis { get; set; }

    /// <summary>
    /// The maximum rotation angle around the Y-axis (in degrees).
    /// </summary>
    public static float MaxRotationYAxis { get; set; }

    /// <summary>
    /// The speed at which the camera zooms in and out.
    /// </summary>
    public static float ZoomSpeed { get; set; }

    /// <summary>
    /// The closest zoom distance.
    /// </summary>
    public static float MinZoomDistance { get; set; }  // Closest zoom distance

    /// <summary>
    /// The farthest zoom distance.
    /// </summary>
    public static float MaxZoomDistance { get; set; } // Farthest zoom distance

    /// <summary>
    /// A constant for scaling mouse movement to camera rotation.
    /// </summary>
    private const float RotationSensitivity = 0.005f;

    /// <summary>
    /// Initializes the camera helper with the main camera and character.
    /// </summary>
    /// <param name="mainCamera">The main camera.</param>
    /// <param name="character">The character associated with the camera.</param>
    public static void Initialize(Camera3D mainCamera, CharacterBody3D character)
    {
        _mainCamera = mainCamera ?? throw new ArgumentNullException(nameof(mainCamera));
        _character = character ?? throw new ArgumentNullException(nameof(character));
    }

    /// <summary>
    /// Handles camera rotation based on mouse movement.
    /// </summary>
    /// <param name="event">The input event, typically from unhandled input.</param>
    public static void HandlePlayerCameraRotation(InputEvent @event)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Middle) && @event is InputEventMouseMotion ev)
        {
            RotateCamera(ev.Relative);
        }
    }

    /// <summary>
    /// Handles camera zooming based on mouse wheel input.
    /// </summary>
    /// <param name="event">The input event, typically from unhandled input.</param>
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
    /// Rotates the camera based on mouse movement.
    /// </summary>
    /// <param name="relativeMovement">The relative movement of the mouse.</param>
    private static void RotateCamera(Vector2 relativeMovement)
    {
        // Rotate Y (left/right)
        float newYRotation = _mainCamera.Rotation.Y - relativeMovement.X * RotationSensitivity;
        newYRotation = Mathf.Clamp(newYRotation, Mathf.DegToRad(MinRotationYAxis), Mathf.DegToRad(MaxRotationYAxis));

        // Rotate X (up/down) but clamp the angle
        float newXRotation = _mainCamera.Rotation.X - relativeMovement.Y * RotationSensitivity;
        newXRotation = Mathf.Clamp(newXRotation, Mathf.DegToRad(MinRotationXAxis), Mathf.DegToRad(MaxRotationXAxis));

        _mainCamera.Rotation = new Vector3(newXRotation, newYRotation, _mainCamera.Rotation.Z);
    }

    /// <summary>
    /// Adjusts the camera's zoom level.
    /// </summary>
    /// <param name="zoomAmount">The amount to zoom in or out.</param>
    private static void AdjustZoom(float zoomAmount)
    {
        // Cache the camera transform and character position
        var cameraTransform = _mainCamera.GlobalTransform;
        Vector3 cameraPosition = cameraTransform.Origin;
        Vector3 characterPosition = _character.Position;

        // Get forward direction (negative Z in local space)
        Vector3 forward = -cameraTransform.Basis.Z;

        // Calculate new position
        Vector3 newPosition = new Vector3(cameraPosition.X, cameraPosition.Y + forward.Y * zoomAmount, cameraPosition.Z);

        // Clamp distance to prevent extreme zooming
        float distance = (newPosition - characterPosition).Length();
        if (distance < MinZoomDistance || distance > MaxZoomDistance)
            return; // Don't apply zoom if out of bounds

        _mainCamera.GlobalTransform = new Transform3D(cameraTransform.Basis, newPosition);

    }

}