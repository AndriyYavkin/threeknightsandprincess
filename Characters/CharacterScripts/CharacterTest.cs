using System.Diagnostics;
using System.Reflection.Emit;
using GameHelperCharacters;
using Godot;

namespace GameCharacters;
public partial class CharacterTest : CharacterBody2D
{
	[Export] float Speed = 300.0f;

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		if(Input.IsMouseButtonPressed(MouseButton.Left))
		{
			velocity = MouseHelper.HandlePlayerMovementToMouse(GetGlobalMousePosition(), GlobalPosition, Speed);
		} 
		else if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
