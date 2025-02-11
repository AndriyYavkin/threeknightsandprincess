using Godot;
using GameHelperCharacters;
using System.Collections.Generic;
using Godot.Collections;

namespace Characters;

public partial class CharacterTest3D : CharacterBody3D
{
	[Export] float Speed = 5.0f; // Speed of character, not in meters!
	[Export] Camera3D mainCamera; // Sets the main camera to change it's position and rotation
	[Export] float zoomSpeed = 0.5f; // Speed of zooming

	public Vector3I GridPosition { get; set; }
	public int MovementRange { get; set; } = 10; // Example movement range
    public int RemainingMovement { get; set; }

	private PackedScene marker;
	private Tween _tween; // for smooth effect?
	private Node3D _spawnedInstance;
	private Vector3 _targetPosition;
	private bool _isMoving = false;
	private bool _isHolding = false; // Track if the mouse is held down

	//As node enters, we init new instance of CharacterHelper to set mainCamera
    public override void _Ready()
    {
		InitializeCharacterHelper();

		marker = GD.Load<PackedScene>("res://3DScenes/Scenes/TestMarker.tscn");
		RemainingMovement = MovementRange;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(Input.IsActionJustPressed("Quit"))
		{
			GetTree().Quit();
		}

		if(Input.IsActionJustPressed("TestTree"))
		{
			GetTree().ChangeSceneToFile("3DScenes/Scenes/battle.tscn");
		}

		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
        {
            if (mouseEvent.Pressed)
            {
                _isHolding = true;
                CharacterHelper.HandleCharacterMovements(GetViewport().GetMousePosition(), GetWorld3D().DirectSpaceState); // Set target position when clicking
            }
            else
            {
                _isHolding = false; // Stop updating target when released
            }
        }

		CharacterHelper.HandlePlayerCameraRotation(@event);
		CharacterHelper.HandleZooming(@event, zoomSpeed, Position);
    }

	public override void _PhysicsProcess(double delta)
	{
		Velocity = CharacterHelper.HandlePlayerMovementsPhysics();
		MoveAndSlide();
	}

	private static bool IsPositionValid(Vector3I position)
    {
        return position.X >= -Scenes.GlobalMap.MapWidth/2 && position.X < Scenes.GlobalMap.MapWidth/2 &&
               position.Z >= -Scenes.GlobalMap.MapHeight/2 && position.Z < Scenes.GlobalMap.MapHeight/2;
    }

	private void InitializeCharacterHelper()
	{
		CharacterHelper.mainCamera = mainCamera;
		// Better to keep y -60 and 60 and x -90 and -60
		CharacterHelper.MinRotationYAxis = -60;
		CharacterHelper.MinRotationXAxis = -90;
		CharacterHelper.MaxRotationYAxis = 60;
		CharacterHelper.MaxRotationXAxis = -60;

		CharacterHelper.Speed = Speed;
		CharacterHelper.Character = this;
	}
}
