using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ScenesHelper.TileMapScripts;

/// <summary>
/// Represents a tile-based map in the game. This class initializes the grid and visualizes tiles based on the GridMap.
/// </summary>
public partial class TilesMap : Node3D, IMapInitializable
{
    /// <summary>
    /// The GridMap node used to define the map's layout and cell size.
    /// </summary>
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

    /// <summary>
    /// A dictionary to map tile names to TileType.
    /// </summary>
    private static readonly Dictionary<string, TileType> TileTypeMapping = new()
    {
        { "Grass", TileType.Grass },
        { "Road", TileType.Road },
        { "Water", TileType.Water },
        { "Mountain", TileType.Mountain },
        { "Forest", TileType.Forest },
        { "Town", TileType.Town }
    };

    /// <summary>
    /// Initializes the grid by iterating over each cell in the GridMap and creating corresponding tiles.
    /// </summary>
    private void InitializeGrid()
    {
        if (GridMap == null || GridMap.MeshLibrary == null)
        {
            GD.PrintErr("GridMap or MeshLibrary is not assigned!");
            return;
        }

        // Iterate over each cell in the GridMap
        Parallel.For(0, MapWidth, x =>
        {
            for (int z = 0; z < MapHeight; z++)
            {
                int tileId = GridMap.GetCellItem(new Vector3I(x, 0, z));

                if (tileId == -1)
                {
                    Scenes.TileMap.Map[x, z] = new Tile(TileType.NotDefined);
                    VisualizeTile(x, z, Scenes.TileMap.Map[x, z]);
                    continue;
                }

                string tileName = GridMap.MeshLibrary.GetItemName(tileId);
                if (string.IsNullOrEmpty(tileName))
                {
                    GD.PrintErr($"Tile at ({x}, {z}) has no name in MeshLibrary!");
                    Scenes.TileMap.Map[x, z] = new Tile(TileType.NotDefined);
                    VisualizeTile(x, z, Scenes.TileMap.Map[x, z]);
                    continue;
                }

                TileType type = TileTypeMapping.TryGetValue(tileName, out var tileType) ? tileType : TileType.NotDefined;
                GD.Print($"Tile at ({x}, {z}) is of type: {type}");

                Scenes.TileMap.Map[x, z] = new Tile(type);
                VisualizeTile(x, z, Scenes.TileMap.Map[x, z]);
            }
        });
    }

    /// <summary>
    /// Visualizes a tile in 3D space by positioning its mesh and adding it to the scene.
    /// </summary>
    /// <param name="x">The x-coordinate of the tile in the grid.</param>
    /// <param name="z">The z-coordinate of the tile in the grid.</param>
    /// <param name="tile">The tile to visualize.</param>
    private void VisualizeTile(int x, int z, Tile tile)
    {
        // Position the tile in 3D space
        tile.PositionGrid = new Vector3I(x, 0, z);
        tile.Position = new Vector3(x * GridPositionConverter, 0, z* GridPositionConverter);
        tile.TileMesh.Position = new Vector3(x * GridPositionConverter, 0, z * GridPositionConverter);

        // Use CallDeferred to add the child on the main thread
        CallDeferred("add_child", tile.TileMesh);
    }
}
