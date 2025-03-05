namespace Game.ScenesHelper;

/// <summary>
/// Represents the types of tiles available in the game.
/// </summary>
/// <remarks>
/// To add a new tile type, define it here and then implement its characteristics in the <see cref="Tile"/> class.
/// For example, to add "Lava," define it here and then specify its properties (e.g., passability, movement cost) in the Tile class.
/// </remarks>
public enum TileType
{
    /// <summary>
    /// Represents an undefined or placeholder tile.
    /// </summary>
    NotDefined,

    /// <summary>
    /// Represents a grass tile, typically passable with a default movement cost.
    /// </summary>
    Grass,

    /// <summary>
    /// Represents a water tile, typically impassable.
    /// </summary>
    Water,

    /// <summary>
    /// Represents a mountain tile, typically impassable.
    /// </summary>
    Mountain,

    /// <summary>
    /// Represents a forest tile, typically passable with a higher movement cost.
    /// </summary>
    Forest,

    /// <summary>
    /// Represents a town tile, typically passable and may have special properties.
    /// </summary>
    Town,

    /// <summary>
    /// Represents a road tile, typically passable with a lower movement cost.
    /// </summary>
    Road,
}