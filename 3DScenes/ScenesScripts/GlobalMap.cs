using Godot;
using ScenesHelper;
using GameHelperCharacters;
using System;
using Characters;

namespace Scenes;

public partial class GlobalMap : Node3D
{
	[Export] Characters.CharacterTest3D hero;

	public const int MapWidth = 50; // Example map size
    public const int MapHeight = 50;
    public const int MapDepth = 1; // For a flat 3D map, depth can be 1
    public static Tile[,,] map { get; set; } 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = new Tile[MapWidth, 1, MapHeight];
		try
		{
			InitializeGrid();
			Pathfinder3D.Initialize(map);
		}
		catch (Exception ex)
		{
			GD.PrintErr("Map failed to initialize! ", ex);
		}

		hero.GlobalPosition = new Vector3(0, 1, 0);
	}

	private static void InitializeGrid()
    {
        for (int x = 0; x < MapWidth; x++)
        {
			for (int z = 0; z < MapHeight; z++)
			{
				map[x, 0, z] = new Tile(TileType.Grass);
			}
		}
    }
}
