using Godot;

namespace Game.ScenesHelper;

/// <summary>
/// Represents a tile in a grid-based system, containing properties and behaviors for different tile types.
/// </summary>
public class Tile
{
    /// <summary>
    /// Gets or sets the type of the tile, which determines its properties and behavior.
    /// </summary>
    public TileType Type { get; private set; }

    /// <summary>
    /// Gets or sets the 3D object associated with the tile. This can be a resource, entity, or decoration.
    /// </summary>
    public Node3D ContainsObject { get; set; }

    /// <summary>
    /// Gets or sets the position of the tile in grid coordinates.
    /// </summary>
    public Vector3I PositionGrid { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tile represents an entity.
    /// </summary>
    public bool IsEntity { get; set; }

    /// <summary>
    /// Indicates whether the tile contains an interactable object.
    /// </summary>
    public bool HasInteractableObject { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tile can be passed through by entities.
    /// </summary>
    public bool IsPassable { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tile"/> class with the specified tile type.
    /// </summary>
    /// <param name="type">The type of the tile.</param>
    public Tile(TileType type)
    {
        Type = type;
        IsPassable = TileProperties.IsPassable(type);
    }

    /// <summary>
    /// Resets the tile to its default state, clearing any associated objects and hiding the tile mesh.
    /// </summary>
    public void Reset()
    {
        ContainsObject = null;
        IsEntity = false;
    }

    /// <summary>
    /// Sets whether the tile is passable.
    /// </summary>
    /// <param name="isPassable">True if the tile is passable; otherwise, false.</param>
    public void SetPassable(bool isPassable) => IsPassable = isPassable;

    /// <summary>
    /// Retrieves whether the tile is passable based on its type.
    /// </summary>
    /// <returns>True if the tile is passable; otherwise, false.</returns>
    public bool GetPassable() => TileProperties.IsPassable(Type);

    /// <summary>
    /// Retrieves the movement cost for this tile based on its type.
    /// </summary>
    /// <returns>The movement cost of the tile.</returns>
    public float GetMovementCost() => TileProperties.GetMovementCost(Type);
}