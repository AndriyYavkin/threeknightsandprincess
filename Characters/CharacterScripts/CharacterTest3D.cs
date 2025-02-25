using Godot;
using GameHelperCharacters;
using ScenesHelper;

namespace Characters;

/// <summary>
/// Represents a 3D character in the game. Handles movement, camera control, and interactions.
/// </summary>
public partial class CharacterTest3D : CharacterBody3D, ICharacterTemplate
{
	/// <summary>
    /// The speed of the character. Not in meters!
    /// </summary>
	[Export] float Speed = 5.0f;
	/// <summary>
    /// The main camera used for player perspective.
    /// </summary>
	[Export] Camera3D mainCamera;
	/// <summary>
    /// The speed of zooming the camera.
    /// </summary>
	[Export] float zoomSpeed = 0.5f;
	/// <summary>
    /// The movement range of the character. Not used!
    /// </summary>
	[Export] int MovementRange { get; set; } = 50;

	public Inventory Inventory { get; set; }

	public Vector3I GridPosition { get; set; }

	/// <summary>
    /// The remaining movement points for the character. Not used!
    /// </summary>
    public int RemainingMovement { get; set; }
	
	private Tween _tween; // for smooth effect
	private bool _isHolding = false; // Track if the mouse is held down

	// TODO: Make visualy acceptable markers
	private static readonly PackedScene _pathMarkerScene = GD.Load<PackedScene>("res://3DScenes/Scenes/PathMarker.tscn");

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
        HandleQuitInput(@event);
        HandleInventoryInput(@event);
        HandleSceneChangeInput(@event);
        HandleMovementInput(@event);
        HandleCameraInput(@event);
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

	/// <summary>
    /// Gets the current tile the character is standing on.
    /// </summary>
    /// <returns>The current tile, or null if out of bounds.</returns>
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

	/// <summary>
    /// Handles the "Quit" input action.
    /// </summary>
    /// <param name="event">The input event.</param>
    private void HandleQuitInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("Quit"))
        {
            GetTree().Quit();
        }
    }

	/// <summary>
    /// Handles the "Inventory" input action.
    /// </summary>
    /// <param name="event">The input event.</param>
    private void HandleInventoryInput(InputEvent @event)
    {
        if (Input.IsKeyPressed(Key.I))
        {
            Inventory.DebugPrint();
        }
    }

    /// <summary>
    /// Handles the "TestTree" input action.
    /// </summary>
    /// <param name="event">The input event.</param>
    private void HandleSceneChangeInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("TestTree"))
        {
            GetTree().ChangeSceneToFile("3DScenes/Scenes/battle.tscn");
        }
    }

	/// <summary>
    /// Handles the camera input, such as rotation and zooming.
    /// </summary>
    /// <param name="event">The input event.</param>
    private void HandleCameraInput(InputEvent @event)
    {
        GameHelperCamera.HandlePlayerCameraRotation(@event);
        GameHelperCamera.HandleZooming(@event);
    }

	/// <summary>
    /// Handles the character movement input.
    /// </summary>
    /// <param name="event">The input event.</param>
    private void HandleMovementInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
        {
            CharacterHelper.HandleCharacterMovements(GetViewport().GetMousePosition(), GetWorld3D().DirectSpaceState);
        }
    }

	/// <summary>
    /// Initializes the CharacterHelper with the character's properties.
    /// </summary>
	private void InitializeCharacterHelper()
	{
		if (mainCamera == null)
        {
            GD.PrintErr("MainCamera is not assigned!");
            return;
        }

		CharacterHelper.Initialize(mainCamera, this, _pathMarkerScene);

		CharacterHelper.Speed = Speed;
	}

	/// <summary>
    /// Initializes the GameHelperCamera with the camera's properties.
    /// </summary>
	private void InitializeCameraHelper()
	{
		if (mainCamera == null)
        {
            GD.PrintErr("MainCamera is not assigned!");
            return;
        }
		GameHelperCamera.Initialize(mainCamera, this);

		// Better to keep y -60 and 60 and x -90 and -60
		GameHelperCamera.MinRotationYAxis = -60;
		GameHelperCamera.MinRotationXAxis = -90;
		GameHelperCamera.MaxRotationYAxis = 60;
		GameHelperCamera.MaxRotationXAxis = -60;
		GameHelperCamera.MinZoomDistance = 2f;
		GameHelperCamera.MaxZoomDistance = 7f;
		GameHelperCamera.ZoomSpeed = zoomSpeed;
	}
}
