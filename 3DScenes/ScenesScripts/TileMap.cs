using Godot;
using ScenesHelper;
using GameHelperCharacters;
using ScenesHelper.TileMapScripts;
using System;

namespace Scenes;

public partial class TileMap : Node3D
{
	[Export] public int MapWidth { get; set; } = 50; // Example map size
    [Export] public int MapHeight { get; set; } = 50;
	[Export] public float GridPositionConverter { get; set; } = 2f;	
    public static Tile[,] Map { get; set; } 


	public override void _Ready()
	{	
		InitializeMap();
		InitializeChildren();
		Pathfinder3D.Initialize(Map);
	}
	
	private void InitializeMap()
    {
        Map = new Tile[MapWidth, MapHeight];

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                // Example: Randomly assign tile types for demonstration
                TileType type = (TileType)(GD.Randi() % Enum.GetValues(typeof(TileType)).Length);
                Map[x, y] = new Tile(type);

                // Position the tile in 3D space
                Map[x, y].TileMesh.Position = new Vector3(x * GridPositionConverter, 0, y * GridPositionConverter);
                AddChild(Map[x, y].TileMesh);
            }
        }
    }

	private void InitializeChildren()
	{
		InitializeStaticHelpers();

		foreach (Node child in GetChildren())
        {
            if (child is ScenesHelper.TileMapScripts.IMapInitializable initializable)
            {
                // Initialize the child with the Tile's values
                initializable.Initialize(MapWidth, MapHeight, GridPositionConverter);
                GD.Print($"Initialized {child.Name} with MapWidth={MapWidth}, MapHeight={MapHeight}, GridPositionConverter={GridPositionConverter}");
            }
        }
	}

	private void InitializeStaticHelpers()
	{
		TileMapHelper.MapWidth = MapWidth;
		TileMapHelper.MapHeight = MapHeight;
		TileMapHelper.GridPositionConverter = GridPositionConverter;

		CharacterHelper.MapWidth = MapWidth;
		CharacterHelper.MapHeight = MapHeight;
		CharacterHelper.GridPositionConverter = GridPositionConverter;
	}
}
