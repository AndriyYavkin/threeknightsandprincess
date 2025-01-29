using Godot;
using GameHelperCharacters;

public partial class CharacterTest3D : CharacterBody3D
{
	[Export] float Speed = 5.0f; // Speed of character, not in meters!
	[Export] Camera3D mainCamera; // Sets the main camera to change it's position and rotation
	[Export] float zoomSpeed = 0.5f; // Speed of zooming

    public override void _UnhandledInput(InputEvent @event)
    {
        if(Input.IsActionJustPressed("Quit"))
		{
			GetTree().Quit();
		}
		
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

		if (@event is InputEventMouseButton mouseEvent)
    	{
			if (mouseEvent.ButtonIndex == MouseButton.WheelUp) // Zoom In
			{
				AdjustZoom(-zoomSpeed);
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelDown) // Zoom Out
			{
				AdjustZoom(zoomSpeed);
			}
    	}
		GD.Print(mainCamera.Rotation);
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		} 
		else if(Input.IsMouseButtonPressed(MouseButton.Left))
		{
			Vector2 mouseViewport = GetViewport().GetMousePosition();
			velocity = MouseHelper.HandlePlayerMovementToMouse(mainCamera, mouseViewport, Speed);
		} 
		else if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void AdjustZoom(float zoomAmount)
	{
		/*GD.Print("Main camera position: ",mainCamera.Position);
		GD.Print("Distance from camera to character: ", mainCamera.Position.DistanceTo(Position));
		GD.Print("Position of character", Position);
		Vector3 tempPositionOfCamera =
		if(mainCamera.Position.DistanceTo(Position) > 5f && mainCamera.Position.DistanceTo(Position) < 10f)
		{
			mainCamera.Position = new Vector3(mainCamera.Position.X, mainCamera.Position.Y + zoomAmount, mainCamera.Position.Z);
		}
		else
		{
			mainCamera.Position = new Vector3(mainCamera.Position.X, );
		}*/
	}
}
