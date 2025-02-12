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

	private Tween _tween; // for smooth effect?
	private bool _isHolding = false; // Track if the mouse is held down
	private PackedScene _pathMarkerScene;

	//As node enters, we init new instance of CharacterHelper to set mainCamera
    public override void _Ready()
    {
		InitializeCharacterHelper();

		// TODO: Make visualy acceptable markers
		_pathMarkerScene = GD.Load<PackedScene>("res://3DScenes/Scenes/PathMarker.tscn");

		CharacterHelper.InitializePathVisualization(_pathMarkerScene);

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
            CharacterHelper.HandleCharacterMovements(GetViewport().GetMousePosition(), GetWorld3D().DirectSpaceState); // Set target position when clicking
        }

		CharacterHelper.HandlePlayerCameraRotation(@event);
		CharacterHelper.HandleZooming(@event);
    }

	public override void _PhysicsProcess(double delta)
	{
		Velocity = CharacterHelper.HandlePlayerMovementsPhysics();
		//CharacterHelper.Update(delta); // if we will need to remove player's choice of path, uncomment this method
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
		CharacterHelper.ZoomSpeed = zoomSpeed;

		CharacterHelper.Speed = Speed;
		CharacterHelper.Character = this;
	}
}
