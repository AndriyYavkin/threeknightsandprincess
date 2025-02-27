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
    public Node3D ContainsObject { get; set; }

    /// <summary>
    /// The mesh instance representing the tile's visual appearance.
    /// </summary>
    public MeshInstance3D TileMesh { get; set; }

    /// <summary>
    /// Indicates whether the tile represents an entity.
    /// </summary>
    public bool IsEntity { get; set; }

    /// <summary>
    /// Indicates whether the tile can be passed through by entities.
    /// </summary>
    public bool IsPassable { get; private set; }

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
        IsPassable = TileProperties.IsPassable(type);
    }

    /// <summary>
    /// Resets the tile to its default state.
    /// </summary>
    public void Reset()
    {
        ContainsObject = null;
        IsEntity = false;
        TileMesh.Visible = false;
    }

    /// <summary>
    /// Sets whether the tile is passable.
    /// </summary>
    /// <param name="isPassable">True if the tile is passable; otherwise, false.</param>
    public void SetPassable(bool isPassable) => IsPassable = isPassable;

    /// <summary>
    /// Retrieves whether the tile is passable.
    /// </summary>
    /// <returns>The passability of the tile.</returns>>
    public bool GetPassable() => TileProperties.IsPassable(Type);

    /// <summary>
    /// Retrieves the movement cost for this tile.
    /// </summary>
    /// <returns>The movement cost of the tile.</returns>
    public float GetMovementCost() => TileProperties.GetMovementCost(Type);
}