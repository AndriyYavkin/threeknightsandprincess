using Godot;
using GameHelperCharacters;
using System;

public partial class Camera3dTesting : Camera3D
{
	[Export] Camera3D camera3D;

	public override void _Ready()
	{
		CharacterHelper.mainCamera = camera3D;
		CharacterHelper.MinRotationYAxis = -360;
		CharacterHelper.MinRotationXAxis = -360;
		CharacterHelper.MaxRotationYAxis = 360;
		CharacterHelper.MaxRotationXAxis = 360;
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        CharacterHelper.HandlePlayerCameraRotation(@event);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
