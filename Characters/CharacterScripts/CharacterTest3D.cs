using Godot;
using Game.HelperCharacters;

namespace Game.Characters;

/// <summary>
/// Represents a 3D character in the game.
/// </summary>
public partial class CharacterTest3D : MainCharacterTemplate
{
    /// <summary>
    /// Gets or sets the speed of zooming the camera.
    /// </summary>
    [Export] public float zoomSpeed { get; set; }

	//As node enters, we init new instance of CharacterHelper to set mainCamera
    public override void _Ready()
    {
        base._Ready();

		InitializeCameraHelper();
    }

    public override void _UnhandledInput(InputEvent @event)
    {        
        base._UnhandledInput(@event);
        HandleQuitInput();
        HandleSceneChangeInput();
    }

	/// <summary>
    /// Handles the "Quit" input action.
    /// </summary>
    /// <param name="event">The input event.</param>
    private void HandleQuitInput()
    {
        if (Input.IsActionJustPressed("Quit"))
        {
            GetTree().Quit();
        }
    }

    /// <summary>
    /// Handles the "TestTree" input action.
    /// </summary>
    /// <param name="event">The input event.</param>
    private void HandleSceneChangeInput()
    {
        if (Input.IsActionJustPressed("TestTree"))
        {
            GetTree().ChangeSceneToFile("3DScenes/Scenes/battle.tscn");
        }
    }

	/// <summary>
    /// Initializes the GameHelperCamera with the camera's properties.
    /// </summary>
	private void InitializeCameraHelper()
	{
		if (MainCamera == null)
        {
            GD.PrintErr("MainCamera is not assigned!");
            return;
        }
		GameHelperCamera.Initialize(MainCamera, this);

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
