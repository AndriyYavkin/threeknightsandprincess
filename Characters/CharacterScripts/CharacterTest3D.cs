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
		CharacterHelper.mainCamera = mainCamera;
		// Better to keep y -60 and 60 and x -90 and -60
		CharacterHelper.MinRotationYAxis = -60;
		CharacterHelper.MinRotationXAxis = -90;
		CharacterHelper.MaxRotationYAxis = 60;
		CharacterHelper.MaxRotationXAxis = -60;

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
		Vector3 velocity = Velocity;

		// Handle movement toward target position
		if (_isMoving)
		{
			Vector3 direction = new Vector3(
            _targetPosition.X - GlobalPosition.X,
            0, // Ignore Y-axis
            _targetPosition.Z - GlobalPosition.Z
        	).Normalized();

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
			// Stop movement if no target
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
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
		Vector3 from = mainCamera.ProjectRayOrigin(mousePos);
		Vector3 to = from + mainCamera.ProjectRayNormal(mousePos) * 100f; // Ray length

		PhysicsDirectSpaceState3D space = GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to);
		Dictionary result = space.IntersectRay(query);

		if (result.Count > 0)
		{
			// Snap the target position to the grid
			var hitPosition = (Vector3)result["position"];
			var targetGridPosition = new Vector3I(
				Mathf.RoundToInt(hitPosition.X / 2), // Adjust scaling as needed
				0,
				Mathf.RoundToInt(hitPosition.Z / 2)
			);
			GD.Print(targetGridPosition);

			// Check if the target position is valid and passable
			if (IsPositionValid(targetGridPosition) && Scenes.GlobalMap.map[targetGridPosition.X, 0, targetGridPosition.Z].IsPassable)
			{
				// Calculate path using the pathfinder
				var path = Pathfinder3D.FindPath(new Vector3I(
					Mathf.RoundToInt(GlobalPosition.X / 2),
					0,
					Mathf.RoundToInt(GlobalPosition.Z / 2)
				), targetGridPosition);

				if (path.Count > 0 && path.Count <= RemainingMovement)
				{
					_targetPosition = new Vector3(targetGridPosition.X * 2, 0, targetGridPosition.Z * 2); // Convert back to world coordinates
					_isMoving = true;
					//RemainingMovement -= path.Count; // Deduct movement points

					// Update marker position
					if (_spawnedInstance == null)
					{
						_spawnedInstance = marker.Instantiate<Node3D>();
						GetTree().CurrentScene.AddChild(_spawnedInstance);
					}
					_spawnedInstance.GlobalPosition = _targetPosition;

					GridPosition = targetGridPosition;
				}
				else
				{
					GD.Print("Not enough movement points or target is unreachable!");
				}
			}
			else
			{
				GD.Print("Cannot move to this tile!");
			}
		}
	}

	private static bool IsPositionValid(Vector3I position)
    {
        return position.X >= 0 && position.X < Scenes.GlobalMap.MapWidth &&
               position.Z >= 0 && position.Z < Scenes.GlobalMap.MapDepth;
    }
}
