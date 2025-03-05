using Godot;
using Game.ScenesHelper;

namespace Game.HelperCharacters;

/// <summary>
/// Represents the base template for a main character, including movement, interaction, and state management.
/// </summary>
public partial class MainCharacterTemplate : CharacterBody3D, ICharacterTemplate
{
    /// <summary>
    /// Gets or sets the speed of the character.
    /// </summary>
    [Export] public float Speed { get; set; } = 5.0f;

    /// <summary>
    /// Gets or sets the main camera used for player perspective.
    /// </summary>
    [Export] public Camera3D MainCamera { get; set; }

    /// <summary>
    /// Gets or sets the scene used for path markers.
    /// </summary>
    [Export] public PackedScene PathMarkerScene { get; set; }

    /// <summary>
    /// Gets or sets character's name
    /// </summary>
    [Export] public string CharacterName { get; set; }

    /// <summary>
    /// The character helper that manages movement, interaction, and state.
    /// </summary>
    protected CharacterHelper CharacterHelper;

    /// <summary>
    /// Gets or sets the character's inventory.
    /// </summary>
    public Inventory Inventory { get; set; }

    /// <summary>
    /// Gets or sets the current grid position of the character.
    /// </summary>
    public Vector3I GridPosition { get; set; }

    private Tween _tween; // for smooth effect
	private bool _isHolding = false; // Track if the mouse is held down

    public string GetTitleUI() => "Character Info";
    public string GetNameUI() => CharacterName;
    public Texture2D GetIconUI() => new Texture2D();/*GD.Load<Texture2D>("res://Textures/Character.png")*/
    public string GetDescriptionUI() => "Some description. Big big big big big big big description.";

    /// <summary>
    /// Called when the node enters the scene tree. Initializes the character.
    /// </summary>
    public override void _Ready()
    {
        Inventory = new Inventory();
        CharacterHelper = new CharacterHelper(MainCamera, this, PathMarkerScene);
    }

    /// <summary>
    /// Handles physics-based movement for the character.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame.</param>
    public override void _PhysicsProcess(double delta)
    {
        CharacterHelper.HandleMovementPhysics();
        MoveAndSlide();

        var currentTile = GetCurrentTile();
        if (currentTile != null && currentTile.ContainsObject != null)
        {
            CharacterHelper.PickUpItem(currentTile);
        }
    }

    /// <summary>
    /// Handles unhandled input events, such as movement and interaction.
    /// </summary>
    /// <param name="event">The input event.</param>
    public override void _UnhandledInput(InputEvent @event)
    {   
        HandleInventoryInput();
        HandleCameraInput(@event);

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
        {
            CharacterHelper.HandleInput(GetViewport().GetMousePosition(), GetWorld3D().DirectSpaceState);
        }
    }

    /// <summary>
    /// Gets the current tile the character is standing on.
    /// </summary>
    /// <returns>The current tile, or null if out of bounds.</returns>
    public Tile GetCurrentTile()
    {
        var gridPosition = new Vector3I(
            (int)Mathf.Round(GlobalPosition.X / CharacterHelper.GridPositionConverter),
            0,
            (int)Mathf.Round(GlobalPosition.Z / CharacterHelper.GridPositionConverter)
        );

        if (gridPosition.X >= 0 && gridPosition.X < Scenes.TileMap.Map.GetLength(0) &&
            gridPosition.Z >= 0 && gridPosition.Z < Scenes.TileMap.Map.GetLength(1))
        {
            return Scenes.TileMap.Map[gridPosition.X, gridPosition.Z];
        }

        return null;
    }

    /// <summary>
    /// Handles the "Inventory" input action.
    /// </summary>
    /// <param name="event">The input event.</param>
    private void HandleInventoryInput()
    {
        if (Input.IsKeyPressed(Key.I))
        {
            Inventory.DebugPrint();
        }
    }

    /// <summary>
    /// Handles the camera input, such as rotation and zooming.
    /// </summary>
    /// <param name="event">The input event.</param>
    private static void HandleCameraInput(InputEvent @event)
    {
        GameHelperCamera.HandlePlayerCameraRotation(@event);
        GameHelperCamera.HandleZooming(@event);
    }

}