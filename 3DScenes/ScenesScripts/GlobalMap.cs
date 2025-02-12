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
		int halfWidth = MapWidth / 2;
		int halfHeight = MapHeight / 2;

        for (int x = -halfWidth; x < halfWidth; x++)
        {
			for (int z = -halfHeight; z < halfHeight; z++)
			{
				map[x + halfWidth, 0, z + halfHeight] = new Tile(TileType.Grass);
			}
		}
    }
}
