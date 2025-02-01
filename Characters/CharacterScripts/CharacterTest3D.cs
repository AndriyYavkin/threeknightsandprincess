using Godot;
using GameHelperCharacters;

namespace Characters;

public partial class CharacterTest3D : CharacterBody3D
{
	[Export] float Speed = 5.0f; // Speed of character, not in meters!
	[Export] Camera3D mainCamera; // Sets the main camera to change it's position and rotation
	[Export] float zoomSpeed = 0.5f; // Speed of zooming

	//As node enters, we init new instance of CharacterHelper to set mainCamera
    public override void _Ready()
    {
		CharacterHelper.mainCamera = mainCamera;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(Input.IsActionJustPressed("Quit"))
		{
			GetTree().Quit();
		}

		CharacterHelper.HandlePlayerCameraRotation(@event);
		CharacterHelper.HandleZooming(@event, zoomSpeed, Position);
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
			velocity = CharacterHelper.HandlePlayerMovementToMouse(mouseViewport, Speed);
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
}
