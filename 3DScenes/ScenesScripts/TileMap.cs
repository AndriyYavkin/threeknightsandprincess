using Godot;
using ScenesHelper;
using GameHelperCharacters;
using System;

namespace Scenes;

public partial class TileMap : Node3D
{
	[Export] public int MapWidth { get; set; } = 50; // Example map size
    [Export] public int MapHeight { get; set; } = 50;
	[Export] public float GridPositionConverter { get; set; } = 2f;	
    public static Tile[,] map { get; set; } 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		try
		{
			InitializeGrid();
			AssignObjectsToTiles();
			Pathfinder3D.Initialize(map);
			InitializeCharacterHelper();
			VisualizeTiles();
		}
		catch (Exception ex)
		{
			GD.PrintErr("Map failed to initialize! ", ex);
		}
	}

	private void InitializeGrid()
    {
        map = new Tile[MapWidth, MapHeight];

        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                // Randomize tile types for variety
                TileType type = (TileType)GD.RandRange(0, 4); // Randomly assign a tile type
                map[x, z] = new Tile(type);
            }
        }
    }

	private void AssignObjectsToTiles()
    {
        foreach (Node child in GetChildren())
        {
            if (child is Node3D node3D)
            {
                AssignObjectToTile(node3D);
            }
        }
    }

	private void AssignObjectToTile(Node3D obj)
    {
        int gridX = (int)Math.Round(obj.Position.X / GridPositionConverter);
        int gridZ = (int)Math.Round(obj.Position.Z / GridPositionConverter);

        if (gridX >= 0 && gridX < MapWidth && gridZ >= 0 && gridZ < MapHeight)
        {
            var tile = map[gridX, gridZ];
            tile.Objects.Add(obj);
			obj.Position = new Vector3(gridX * 2, 0 , gridZ * 2);
            GD.Print($"Assigned object {obj.Name} to tile at ({gridX}, {gridZ})");
        }
        else
        {
            GD.PrintErr($"Object {obj.Name} is outside the map boundaries!");
        }
    }

	private void VisualizeTiles()
    {
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                var tile = map[x, z];
                // Create a visual representation for the tile
                var meshInstance = new MeshInstance3D();
                meshInstance.Mesh = new BoxMesh();
                meshInstance.MaterialOverride = new StandardMaterial3D
                {
                    AlbedoColor = tile.TileColor
                };
                // Position the tile
                meshInstance.Position = new Vector3(x * GridPositionConverter, 0, z * GridPositionConverter);
                AddChild(meshInstance);
				GD.Print(GetChildCount());
            }
        }
    }

	private void InitializeCharacterHelper()
	{
		CharacterHelper.MapWidth = MapWidth;
		CharacterHelper.MapHeight = MapHeight;
		CharacterHelper.GridPositionConverter = GridPositionConverter;
	}
}
