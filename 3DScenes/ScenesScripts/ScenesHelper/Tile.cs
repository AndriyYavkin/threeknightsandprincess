using Godot;

namespace ScenesHelper;

/// <summary>
/// Represents a tile in a grid-based system.
/// </summary>
public class Tile
{
    /// <summary>
    /// The type of the tile, which determines its properties and behavior.
    /// </summary>
    public TileType Type { get; private set; }

    /// <summary>
    /// The 3D object associated with the tile. Can be a resource or an entity.
    /// </summary>
    public Node3D Object { get; set; } = null;

    /// <summary>
    /// The mesh instance representing the tile's visual appearance.
    /// </summary>
    public MeshInstance3D TileMesh { get; private set; }

    /// <summary>
    /// Indicates whether the tile represents an entity.
    /// </summary>
    public bool IsEntity { get; set; } = false;

    /// <summary>
    /// Indicates whether the tile can be passed through by entities.
    /// </summary>
    public bool IsPassable { get; private set; }


    /// <summary>
    /// The cost of moving through this tile. Higher values indicate more difficult terrain.
    /// </summary>
    public float MovementCost { get; private set; } = 1.0f;

    /// <summary>
    /// The position of the tile in grid coordinates.
    /// </summary>
    public Vector3I PositionGrid { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tile"/> class.
    /// </summary>
    /// <param name="type">The type of the tile.</param>
    /// <param name="texture">Optional texture to apply to the tile.</param>
    public Tile(TileType type)
    {
        Type = type;
        TileMesh = new MeshInstance3D();
        InitializeTileProperties(type);
        InitializeTileMesh();
    }

    public void SetPassable(bool isPassable)
    {
        IsPassable = isPassable;
    }

    /// <summary>
    /// A default material for undefined tiles.
    /// </summary>
    private static readonly StandardMaterial3D DefaultMaterial = new()
    {
        AlbedoColor = new Color(0.5f, 0.5f, 0.5f) // Gray color for undefined tiles
    };

    /// <summary>
    /// Initializes tile properties such as passability and movement cost based on the tile type.
    /// </summary>
    private void InitializeTileProperties(TileType type)
    {
        IsPassable = type != TileType.Water && type != TileType.Mountain && type != TileType.NotDefined;

        MovementCost = type switch
        {
            TileType.Road => 0.75f,
            TileType.Grass => 1.0f,
            TileType.Forest => 1.25f,
            _ => 1.0f // Default cost for other tiles
        };
    }

    /// <summary>
    /// Initializes the tile mesh based on the tile type.
    /// </summary>
    private void InitializeTileMesh()
    {
        if (Type == TileType.NotDefined)
        {
            TileMesh.Mesh = new QuadMesh();
            TileMesh.RotateX(Mathf.DegToRad(-90)); // Rotate to lie flat on the ground
            TileMesh.MaterialOverride = DefaultMaterial;
        }
    }
}