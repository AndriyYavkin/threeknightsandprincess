using Godot;
using GameHelperCharacters;
using System.Collections.Generic;
using ScenesHelper;

namespace Characters;

public partial class CharacterTest3D : CharacterBody3D, ICharacterTemplate
{
	[Export] float Speed = 5.0f; // Speed of character, not in meters!
	[Export] Camera3D mainCamera; // Sets the main camera to change it's position and rotation
	[Export] float zoomSpeed = 0.5f; // Speed of zooming
	[Export] int MovementRange { get; set; } = 50; // Example movement range

	public Inventory Inventory { get; set; }
	public Vector3I GridPosition { get; set; }
    public int RemainingMovement { get; set; }
	
	private Tween _tween; // for smooth effect
	private bool _isHolding = false; // Track if the mouse is held down

	// TODO: Make visualy acceptable markers
	private static PackedScene _pathMarkerScene = GD.Load<PackedScene>("res://3DScenes/Scenes/PathMarker.tscn");

	//As node enters, we init new instance of CharacterHelper to set mainCamera
    public override void _Ready()
    {
		Inventory = new Inventory(); // Ensure Inventory is not null

		InitializeCharacterHelper();
		InitializeCameraHelper();

		RemainingMovement = MovementRange;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(Input.IsActionJustPressed("Quit"))
		{
			GetTree().Quit();
		}

		if(Input.IsKeyPressed(Key.I))
		{
			Inventory.DebugPrint();
		}

		if(Input.IsActionJustPressed("TestTree"))
		{
			GetTree().ChangeSceneToFile("3DScenes/Scenes/battle.tscn");
		}

		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
        {
            CharacterHelper.HandleCharacterMovements(GetViewport().GetMousePosition(), GetWorld3D().DirectSpaceState); // Set target position when clicking
        }

		GameHelperCamera.HandlePlayerCameraRotation(@event);
		GameHelperCamera.HandleZooming(@event);
    }

	public override void _PhysicsProcess(double delta)
	{
		Velocity = CharacterHelper.HandlePlayerMovementsPhysics();
		MoveAndSlide();

		var currentTile = GetCurrentTile();
        if (currentTile != null && currentTile.Object != null)
        {
            CharacterHelper.PickUpItem(currentTile);
        }
	}

	private Tile GetCurrentTile()
    {
        // Calculate the current grid position of the character
        var gridPosition = new Vector3I(
            (int)Mathf.Round(GlobalPosition.X / CharacterHelper.GridPositionConverter),
            0,
            (int)Mathf.Round(GlobalPosition.Z / CharacterHelper.GridPositionConverter)
        );

        // Return the tile at the current grid position
        if (gridPosition.X >= 0 && gridPosition.X < Scenes.TileMap.Map.GetLength(0) &&
            gridPosition.Z >= 0 && gridPosition.Z < Scenes.TileMap.Map.GetLength(1))
        {
            return Scenes.TileMap.Map[gridPosition.X, gridPosition.Z];
        }

        return null;
    }

	private void InitializeCharacterHelper()
	{
		CharacterHelper.mainCamera = mainCamera;
		CharacterHelper.Speed = Speed;
		CharacterHelper.Character = this;
		CharacterHelper.pathMarkerScene = _pathMarkerScene;
	}

	private void InitializeCameraHelper()
	{
		GameHelperCamera.mainCamera = mainCamera;
		// Better to keep y -60 and 60 and x -90 and -60
		GameHelperCamera.MinRotationYAxis = -60;
		GameHelperCamera.MinRotationXAxis = -90;
		GameHelperCamera.MaxRotationYAxis = 60;
		GameHelperCamera.MaxRotationXAxis = -60;
		GameHelperCamera.minZoomDistance = 2f;
		GameHelperCamera.maxZoomDistance = 7f;
		GameHelperCamera.ZoomSpeed = zoomSpeed;
		GameHelperCamera.Character = this;
	}
}
