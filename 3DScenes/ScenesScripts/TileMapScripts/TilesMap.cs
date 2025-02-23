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

        InitializeGrid();
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

                if(tileId == -1)
                {
                    Scenes.TileMap.Map[x, z] = new Tile(TileType.NotDefined);
                    VisualizeTile(x, z, Scenes.TileMap.Map[x, z]);
                    continue;
                }

                string tileName = GridMap.MeshLibrary.GetItemName(tileId);
                // Convert the tile ID to a TileType
                TileType type = tileName switch
                {
                    "Grass" => TileType.Grass,
                    "Road" => TileType.Road,
                    "Water" => TileType.Water,
                    "Mountain" => TileType.Mountain,
                    "Forest" => TileType.Forest,
                    "Town" => TileType.Town,
                    _ => TileType.NotDefined // Default to Grass if the tile ID is unknown
                };
                GD.Print(type);
                // Create a new Tile object and store it in the 3D tiles array
                Scenes.TileMap.Map[x, z] = new Tile(type);

                // Visualize the tile in 3D space
                VisualizeTile(x, z, Scenes.TileMap.Map[x, z]);
            }
        }
    }

    private void VisualizeTile(int x, int z, Tile tile)
    {
        // Position the tile in 3D space
        tile.PositionGrid = new Vector3I(x, 0, z);
        tile.Position = new Vector3(x * GridPositionConverter, 0, z* GridPositionConverter);
        tile.TileMesh.Position = new Vector3(x * GridPositionConverter, 0, z * GridPositionConverter);

        // Add the tile's mesh to the scene
        AddChild(tile.TileMesh);
    }
}
