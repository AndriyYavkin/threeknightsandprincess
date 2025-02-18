using Godot;

namespace ScenesHelper.TileMapScripts;

public partial class TilesMap : Node3D, IMapInitializable
{
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public float GridPositionConverter { get; set; }


	// Called when the node enters the scene tree for the first time.

    public void Initialize(int mapWidth, int mapHeight, float gridPositionConverter)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        GridPositionConverter = gridPositionConverter;

        InitializeGrid();
    }

	private void InitializeGrid()
    {
        Scenes.TileMap.map = new Tile[MapWidth, MapHeight];

        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                // Randomize tile types for variety
                TileType type = (TileType)GD.RandRange(0, 4); // Randomly assign a tile type
                Scenes.TileMap.map[x, z] = new Tile(type);
                VisualizeTile(x, z);
            }
        }
    }

	private void VisualizeTile(int x, int z)
    {

        var tile = Scenes.TileMap.map[x, z];
        // Create a visual representation for the tile
        var meshInstance = new MeshInstance3D();
        meshInstance.Mesh = new BoxMesh();
        meshInstance.MaterialOverride = new StandardMaterial3D
        {
            AlbedoColor = tile.TileColor
        };
        // Position the tile
        meshInstance.Position = new Vector3(x * GridPositionConverter, 0, z * GridPositionConverter);
        Scenes.TileMap.map[x,z].Tilemesh = meshInstance;
        AddChild(Scenes.TileMap.map[x,z].Tilemesh);
    }
}
