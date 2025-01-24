using Godot;
using GameHelperCharacters;

public partial class CharacterTest3D : CharacterBody3D
{
	[Export] float Speed = 5.0f;
	[Export] Camera3D mainCamera;

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
}
