using Godot;
using Godot.Collections;
using UI;

namespace Game.Scenes;

public partial class MainScene : Node3D
{
	private Camera3D _camera;
    private UIHandler _uiHandler;


	public override void _Ready()
	{
		_camera = GetViewport().GetCamera3D();
        _uiHandler = GetNode<UIHandler>("UIHandler");
	}

	public override void _Process(double delta)
	{
		HandleHoverInteraction();
	}

	public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
        {
            HandleRightClick(mouseEvent.Position);
        }
    }

	/// <summary>
    /// Handles right-click interactions.
    /// </summary>
    /// <param name="mousePosition">The position of the mouse click.</param>
    private void HandleRightClick(Vector2 mousePosition)
    {
        var clickedObject = GetObjectUnderMouse(mousePosition);
        GD.Print(clickedObject);
        if (clickedObject != null)
        {
            // Convert the mouse position to the global UI position
            var screenPosition = GetViewport().GetMousePosition();
            _uiHandler.HandleRightClick(clickedObject, screenPosition);
        }
    }

	/// <summary>
    /// Handles hover interactions.
    /// </summary>
    private void HandleHoverInteraction()
    {
        var mousePosition = GetViewport().GetMousePosition();
        var hoveredObject = GetObjectUnderMouse(mousePosition);
        _uiHandler.HandleHoverInteraction(hoveredObject);
    }

	    /// <summary>
    /// Gets the object under the mouse cursor.
    /// </summary>
    /// <param name="mousePosition">The position of the mouse.</param>
    /// <returns>The object under the mouse, or null if no object is found.</returns>
    private Node GetObjectUnderMouse(Vector2 mousePos)
    {
        // Create a ray from the camera
        Vector3 from = _camera.ProjectRayOrigin(mousePos);
        Vector3 to = from + _camera.ProjectRayNormal(mousePos) * 100f; // Ray length

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to);
        Dictionary result = GetWorld3D().DirectSpaceState.IntersectRay(query);

        if (result.Count > 0 && result.TryGetValue("collider", out var colliderVariant))
		{
			// Extract the collider from the Variant and cast it to Node
			if (colliderVariant.VariantType == Variant.Type.Object)
			{
				return colliderVariant.As<Node>();
			}
		}

		return null; // No object found
    }
}