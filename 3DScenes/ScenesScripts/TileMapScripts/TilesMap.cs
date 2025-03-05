using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Game.ScenesHelper.TileMapScripts;

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

    private static readonly StandardMaterial3D DefaultMaterial = new()
    {
        AlbedoColor = new Color(0.5f, 0.5f, 0.5f)
    };

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

        List<Vector3> notDefinedPositions = new();

        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                int tileId = GridMap.GetCellItem(new Vector3I(x, 0, z));
                TileType type = tileId == -1 ? TileType.NotDefined : GetTileTypeFromId(tileId);

                Scenes.TileMap.Map[x, z] = new Tile(type) 
                { 
                    PositionGrid = new Vector3I(x, 0, z) 
                };

                if (type == TileType.NotDefined)
                    notDefinedPositions.Add(new Vector3(x * GridPositionConverter, 0, z * GridPositionConverter));
            }
        }

        VisualizeNotDefinedTiles(notDefinedPositions);
    }

    /// <summary>
    /// Retrieves the TileType corresponding to a tile ID.
    /// </summary>
    /// <param name="tileId">The ID of the tile.</param>
    /// <returns>The TileType corresponding to the tile ID, or TileType.NotDefined if not found.</returns>
    private TileType GetTileTypeFromId(int tileId)
    {
        string tileName = GridMap.MeshLibrary.GetItemName(tileId);
        return TileTypeMapping.TryGetValue(tileName, out var type) ? type : TileType.NotDefined;
    }

    /// <summary>
    /// Visualizes tiles that are marked as "NotDefined" in the grid.
    /// </summary>
    /// <param name="positions">The positions of the "NotDefined" tiles.</param>
    private void VisualizeNotDefinedTiles(List<Vector3> positions)
    {
        if (positions.Count == 0) return;

        var multiMesh = new MultiMesh
        {
            Mesh = new QuadMesh(),
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            InstanceCount = positions.Count
        };

        for (int i = 0; i < positions.Count; i++)
        {
            Transform3D transform = Transform3D.Identity
                .TranslatedLocal(positions[i])
                .RotatedLocal(Vector3.Right, Mathf.DegToRad(-90));
            multiMesh.SetInstanceTransform(i, transform);
        }

        var multiMeshInstance = new MultiMeshInstance3D
        {
            Multimesh = multiMesh,
            MaterialOverride = DefaultMaterial
        };

        AddChild(multiMeshInstance);
    }
}
