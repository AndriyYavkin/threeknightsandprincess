using Godot;
using ScenesHelper;
using GameHelperCharacters;
using System;
using Characters;

namespace Scenes;

public partial class TileMap : Node3D
{
	[Export] public int MapWidth { get; set; } = 50; // Example map size
    [Export] public int MapHeight { get; set; } = 50;
	[Export] public float GridPositionConverter { get; set; } = 2f;	
    public static Tile[,] map { get; set; } 


	public override void _Ready()
	{
		InitializeChildren();
		Pathfinder3D.Initialize(map);
	}
	
	private void InitializeChildren()
	{
		foreach (Node3D child in GetChildren())
        {
            if (child is ScenesHelper.TileMapScripts.IMapInitializable initializable)
            {
                // Initialize the child with the Tile's values
                initializable.Initialize(MapWidth, MapHeight, GridPositionConverter);
                GD.Print($"Initialized {child.Name} with MapWidth={MapWidth}, MapHeight={MapHeight}, GridPositionConverter={GridPositionConverter}");
            }
        }

		CharacterHelper.MapWidth = MapWidth;
		CharacterHelper.MapHeight = MapHeight;
		CharacterHelper.GridPositionConverter = GridPositionConverter;
	}
}
