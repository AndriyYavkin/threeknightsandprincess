using GameHelperCharacters;
using Godot;
using System;
using System.Security.Cryptography;

public partial class TestObject : MeshInstance3D, IItem
{
	public string Name { get; set; }

	public void PickUp(CharacterBody3D character)
	{
		GD.Print("Picked up");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Name = "Name";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
