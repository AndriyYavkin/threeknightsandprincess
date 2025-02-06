using Godot;
using GameHelperCharacters;
using System.Collections.Generic;

namespace Characters;

public partial class CharacterTest3D : CharacterBody3D
{
	[Export] float Speed = 5.0f; // Speed of character, not in meters!
	[Export] Camera3D mainCamera; // Sets the main camera to change it's position and rotation
	[Export] float zoomSpeed = 0.5f; // Speed of zooming

	private PackedScene marker;
	private Tween _tween; // for smooth effect?
	private Node3D _spawnedInstance;
	private Vector3 _targetPosition;
	private bool _isMoving = false;
	private bool _isHolding = false; // Track if the mouse is held down

	//As node enters, we init new instance of CharacterHelper to set mainCamera
    public override void _Ready()
    {
		CharacterHelper.mainCamera = mainCamera;
		// Better to keep y -60 and 60 and x -90 and -60
		CharacterHelper.MinRotationYAxis = -60;
		CharacterHelper.MinRotationXAxis = -90;
		CharacterHelper.MaxRotationYAxis = 60;
		CharacterHelper.MaxRotationXAxis = -60;

		marker = GD.Load<PackedScene>("res://3DScenes/Scenes/TestMarker.tscn");
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

		 if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.Pressed)
                {
                    _isHolding = true;
                    SpawnInstance(); // Set target position when clicking
                }
                else
                {
                    _isHolding = false; // Stop updating target when released
                }
            }
        }

		CharacterHelper.HandlePlayerCameraRotation(@event);
		CharacterHelper.HandleZooming(@event, zoomSpeed, Position);
    }

    public override void _PhysicsProcess(double delta)
	{
		// TODO: Refactor this code and also replace some of the features into the event handler so it won't update each frame
		Vector3 velocity = Velocity;

		// Handle movement toward target position
		if (_isMoving)
		{
			Vector3 direction = (_targetPosition - GlobalPosition).Normalized();
			velocity = direction * Speed;
			// Stop moving when close enough
			if (GlobalPosition.DistanceTo(_targetPosition) < 0.1f)
			{
				velocity = Vector3.Zero;
				_isMoving = false;
			}
		}
		else
		{
			// Handle player input movement
			Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
			Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}
		}
		Velocity = velocity;
		MoveAndSlide();
		}

	private void SpawnInstance()
	{
		if (marker == null || mainCamera == null)
			return;

		Vector2 mousePos = GetViewport().GetMousePosition();

		// Create a ray from the camera
		var from = mainCamera.ProjectRayOrigin(mousePos);
		var to = from + mainCamera.ProjectRayNormal(mousePos) * 100f; // Ray length

		var space = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		var result = space.IntersectRay(query);

		if (result.Count > 0)
		{
			_targetPosition = (Vector3)result["position"];
			_isMoving = true; // Start moving towards the target

			// If the marker doesn't exist, create it
			if (_spawnedInstance == null)
			{
				_spawnedInstance = marker.Instantiate<Node3D>();
				GetTree().CurrentScene.AddChild(_spawnedInstance);
			}

			// Update marker position
			_spawnedInstance.GlobalPosition = _targetPosition;
		}

		GD.Print(GetChildCount());
	}
}
