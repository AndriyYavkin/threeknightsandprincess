using Godot;
using Scenes;

namespace ScenesHelper.TileMapScripts;

public partial class TilesMap : Node3D, IMapInitializable
{
    // [Export] PackedScene TileScene2D;

    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public float GridPositionConverter { get; set; }

    public void Initialize(int mapWidth, int mapHeight, float gridPositionConverter)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        GridPositionConverter = gridPositionConverter;

        //InitializeGrid();
        LoadTileMapFromScene("res://2DScenes/TileMap/tile_map_global_map.tscn");
    }

    private void LoadTileMapFromScene(string scenePath)
    {
        // Load the 2D TileMap scene
        var tileMapScene = GD.Load<PackedScene>(scenePath);
        var tileMapNode = tileMapScene.Instantiate() as TileMapLayer;

        if (tileMapNode == null)
        {
            GD.PrintErr("Failed to load TileMap scene.");
            return;
        }

        var tileSet = tileMapNode.TileSet;

        if (tileSet == null)
        {
            GD.PrintErr("TileMap does not have a TileSet.");
            return;
        }

        // Initialize the map array
        Scenes.TileMap.Map = new Tile[MapWidth, MapHeight];

        // Iterate through all cells in the TileMap
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                var cellPosition = new Vector2I(x, y);
                var tileData = tileMapNode.GetCellTileData(cellPosition);

                if (tileData != null)
                {
                    var terrain = tileData.Terrain;

                    GD.Print($"Cell ({x}, {y}), Terrain = {terrain}");

                    // Map terrain to TileType
                    TileType type = MapTerrainToTileType(terrain);

                    Texture2D texture = GetTextureFromTileSet(tileSet, tileMapNode, new Vector2I(x,y));

                    // Create a new Tile object
                    Scenes.TileMap.Map[x, y] = new Tile(type, texture);

                    // Position the tile in 3D space
                    Scenes.TileMap.Map[x, y].TileMesh.Position = new Vector3(x * GridPositionConverter, 0, y * GridPositionConverter);
                    AddChild(Scenes.TileMap.Map[x, y].TileMesh);
                }
                else
                {
                    // Default to Grass if no tile data is found
                    Scenes.TileMap.Map[x, y] = new Tile(TileType.Grass);
                    Scenes.TileMap.Map[x, y].TileMesh.Position = new Vector3(x * GridPositionConverter, 0, y * GridPositionConverter);
                    AddChild(Scenes.TileMap.Map[x, y].TileMesh);
                }
            }
        }

        // Clean up the TileMap node
        tileMapNode.QueueFree();
    }

    private static Texture2D GetTextureFromTileSet(TileSet tileSet, TileMapLayer tileMapLayer, Vector2I cellPosition)
    {
        // Get the source ID and atlas coordinates for the tile
        var sourceId = tileMapLayer.GetCellSourceId(cellPosition);
        var atlasCoords = tileMapLayer.GetCellAtlasCoords(cellPosition);
        // Get the TileSetSource for the tile
        var tileSetSource = tileSet.GetSource(sourceId) as TileSetAtlasSource;

        GD.Print($"Cell ({cellPosition.X}, {cellPosition.Y}): SourceId = {sourceId}, AtlasCoords = {atlasCoords}"); 

        if (tileSetSource != null)
        {
            // Get the texture region for the tile
            var textureRegion = tileSetSource.GetTileTextureRegion(atlasCoords);

            // Debug: Print texture region information
            GD.Print($"Cell ({cellPosition.X}, {cellPosition.Y}): TextureRegion = {textureRegion}");

            // Return the atlas texture (the entire texture sheet)
            return tileSetSource.Texture;
        }

        return null; // Return null if no texture is found
    }

    private static TileType MapTerrainToTileType(int terrain)
    {
        // Map terrain data to TileType based on your game's logic
        // Example mapping:
        switch (terrain)
        {
            case 0: // Example: Terrain 0 is Grass
                return TileType.Grass;
            case 1: // Example: Terrain 1 is Water
                return TileType.Water;
            case 2: // Example: Terrain 2 is Mountain
                return TileType.Mountain;
            case 3: // Example: Terrain 3 is Forest
                return TileType.Forest;
            case 4: // Example: Terrain 4 is Town
                return TileType.Town;
            default:
                return TileType.Grass; // Default to Grass
        }
    }

	private void InitializeGrid()
    {
        Scenes.TileMap.Map = new Tile[MapWidth, MapHeight];

        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                // Randomize tile types for variety
                TileType type = (TileType)GD.RandRange(0, 4); // Randomly assign a tile type
                Scenes.TileMap.Map[x, z] = new Tile(type);
                VisualizeTile(x, z);
            }
        }
    }

	private void VisualizeTile(int x, int z)
    {

        var tile = Scenes.TileMap.Map[x, z];
        // Create a visual representation for the tile
        var meshInstance = new MeshInstance3D();
        meshInstance.Mesh = new QuadMesh();
        // Position the tile
        meshInstance.Position = new Vector3(x * GridPositionConverter, 0, z * GridPositionConverter);
        Scenes.TileMap.Map[x,z].TileMesh = meshInstance;
        AddChild(Scenes.TileMap.Map[x,z].TileMesh);
    }
}
