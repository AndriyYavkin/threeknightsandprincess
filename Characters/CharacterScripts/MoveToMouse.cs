using Characters;
using Godot;
using Godot.Collections;
using System;
using System.Runtime.CompilerServices;

public partial class MoveToMouse : Node3D
{
	private Camera3D _cam;
	[Export] CharacterTest3D _player;

	[Export] private PackedScene _targetPointPrefab;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_cam = GetViewport().GetCamera3D();
	}

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton b && b.Pressed && b.ButtonIndex == MouseButton.Left)
		{
			var result = RayCast(b.Position, 2);
			if(result != null)
			{
				Vector3 pos = result["position"].AsVector3();
				Node3D targetPosition = _targetPointPrefab.Instantiate<Node3D>();
				AddChild(targetPosition);
				targetPosition.GlobalPosition = pos;
				_player.SetTargetPosition(pos);
			}
		}
    }

    private Dictionary RayCast(Vector2 mousePos, uint collisionMask = 4294967295)
	{
		Vector3 from = _cam.ProjectRayOrigin(mousePos);
		Vector3 to = from + _cam.ProjectRayNormal(mousePos) * 1000f;
		PhysicsRayQueryParameters3D query = new() {
			From = from,
			To = to,
			CollisionMask = collisionMask,
		};
		PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
		var result = spaceState.IntersectRay(query);
		if (result.Count > 1) return result;
		return null;
	}
}
