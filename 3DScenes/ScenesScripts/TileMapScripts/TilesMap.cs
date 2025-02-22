using GameHelperCharacters;
using Godot;
using Scenes;

namespace ScenesHelper.TileMapScripts;

public partial class TilesMap : Node3D, IMapInitializable
{
    [Export] public GridMap GridMap { get; set; } // Assign the GridMap node in the editor
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public float GridPositionConverter { get; set; }

    public void Initialize(int mapWidth, int mapHeight, float gridPositionConverter)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        GridPositionConverter = gridPositionConverter;

        // Initialize the 3D tiles array
        Scenes.TileMap.Map = new Tile[MapWidth, MapHeight];

        // Iterate over each cell in the GridMap
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                // Get the tile ID from the GridMap
                int tileId = GridMap.GetCellItem(new Vector3I(x, 0, y));

                // Convert the tile ID to a TileType
                TileType type = tileId switch
                {
                    0 => TileType.Grass,
                    1 => TileType.Road,
                    2 => TileType.Water,
                    3 => TileType.Mountain,
                    _ => TileType.Grass // Default to Grass if the tile ID is unknown
                };

                // Create a new Tile object and store it in the 3D tiles array
                Scenes.TileMap.Map[x, y] = new Tile(type);

                // Visualize the tile in 3D space
                VisualizeTile(x, y, Scenes.TileMap.Map[x, y]);
            }
        }
    }

    private void InitializeGrid()
    {
        // Iterate over each cell in the GridMap
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                // Get the tile ID from the GridMap
                int tileId = GridMap.GetCellItem(new Vector3I(x, 0, z));

                // Convert the tile ID to a TileType
                TileType type = tileId switch
                {
                    0 => TileType.Grass,
                    1 => TileType.Road,
                    2 => TileType.Water,
                    3 => TileType.Mountain,
                    _ => TileType.Grass // Default to Grass if the tile ID is unknown
                };

                // Create a new Tile object and store it in the 3D tiles array
                Scenes.TileMap.Map[x, z] = new Tile(type);

                // Visualize the tile in 3D space
                VisualizeTile(x, z, Scenes.TileMap.Map[x, z]);
            }
        }
    }

    private void VisualizeTile(int x, int z, Tile tile)
    {
        // Determine which 3D model to use based on the tile type
        PackedScene modelScene = null;
        switch (tile.Type)
        {
            case TileType.Grass:
                GD.Print("Loaded Grass");
                break;
            case TileType.Road:
                GD.Print("Loaded Road");
                break;
            case TileType.Water:
                GD.Print("Loaded Water");
                break;
            case TileType.Mountain:
                GD.Print("Loaded Mountain");
                break;
        }

        if (modelScene != null)
        {
            // Instantiate the 3D model
            var modelInstance = modelScene.Instantiate<MeshInstance3D>();

            // Position the model in 3D space
            modelInstance.Position = new Vector3(x * GridPositionConverter, 0, z * GridPositionConverter);

            // Add the model to the scene
            AddChild(modelInstance);

            // Store the model in the Tile object
            tile.TileMesh = modelInstance;
        }
        tile.TileMesh = null;
    }

	private void InitializeGridLegacy()
    {
        Scenes.TileMap.Map = new Tile[MapWidth, MapHeight];

        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                // Randomize tile types for variety
                TileType type = (TileType)GD.RandRange(0, 4); // Randomly assign a tile type
                Scenes.TileMap.Map[x, z] = new Tile(type);
                VisualizeTileLegacy(x, z);
            }
        }
    }

	private void VisualizeTileLegacy(int x, int z)
    {

        // Create a visual representation for the tile
        var meshInstance = new MeshInstance3D();
        meshInstance.Mesh = new QuadMesh();
        // Position the tile
        meshInstance.Position = new Vector3(x * GridPositionConverter, 0, z * GridPositionConverter);
        Scenes.TileMap.Map[x,z].TileMesh = meshInstance;
        AddChild(Scenes.TileMap.Map[x,z].TileMesh);
    }
}
