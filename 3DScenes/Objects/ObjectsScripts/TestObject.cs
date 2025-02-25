using Godot;

namespace ObjectsScripts;

public partial class TestObject : MeshInstance3D, IItem
{
	public string ItemName { get; set; }

	public void PickUp(CharacterBody3D character)
	{
		GD.Print("Picked up");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ItemName = "Name";
	}
}
