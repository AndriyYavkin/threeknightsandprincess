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

		CharacterHelper.HandlePlayerCameraRotation(@event);
		CharacterHelper.HandleZooming(@event, zoomSpeed, Position);
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		if(Input.IsMouseButtonPressed(MouseButton.Left))
		{
			/*GD.Print("Camera: ",mainCamera.ProjectPosition(GetViewport().GetMousePosition(), Speed));
			GD.Print("Position: ", Position);*/
			SpawnInstance();
		}

		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		//To be honest, in this kind of games, I don't think we will need to be able to move via keys.
		/*if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		} 
		else if(Input.IsMouseButtonPressed(MouseButton.Left))
		{
			Vector2 mouseViewport = GetViewport().GetMousePosition();
			velocity = CharacterHelper.HandlePlayerMovementToMouse(mouseViewport, Speed, Position);
			GD.Print(velocity);
		} 
		else*/ if (direction != Vector3.Zero)
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

	public Vector3 SetTargetPosition(Vector3 pos)
	{
		return Vector3.Zero;
	}

	private void SpawnInstance()
	{
		if (marker == null)
			return;

		if (mainCamera == null)
			return;
		// Get mouse position in viewport
		Vector2 mousePos = GetViewport().GetMousePosition();

		// Create a ray from the camera
		var from = mainCamera.ProjectRayOrigin(mousePos);
		var to = from + mainCamera.ProjectRayNormal(mousePos) * 100f; // Ray length

		var space = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		var result = space.IntersectRay(query);

		if (result.Count > 0)
		{
			// Get the collision point
			Vector3 position = (Vector3)result["position"];

			// If an instance exists, update its position instead of creating a new one
			if (_spawnedInstance == null)
			{
				_spawnedInstance = marker.Instantiate<Node3D>();
				AddChild(_spawnedInstance);
			}

			_spawnedInstance.GlobalPosition = position;

			this.MoveTo(position);
		}
		GD.Print(GetChildCount());
	}

	private void MoveTo(Vector3 targetPosition)
	{
		_tween = CreateTween();	

		// Animate movement to target position over time
		_tween.TweenProperty(this, "global_position", targetPosition, 0.5f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);

		_tween.Play();
	}
}
