using System.Collections.Generic;

namespace ScenesHelper;

/// <summary>
/// Provides properties and behaviors for different tile types.
/// </summary>
public static class TileProperties
{
    private static readonly Dictionary<TileType, bool> PassableLookup = new()
    {
        { TileType.NotDefined, false }, { TileType.Grass, true }, { TileType.Water, false },
        { TileType.Mountain, false }, { TileType.Forest, true }, { TileType.Town, true },
        { TileType.Road, true }
    };

    private static readonly Dictionary<TileType, float> MovementCostLookup = new()
    {
        { TileType.Road, 0.75f }, { TileType.Grass, 1.0f }, { TileType.Forest, 1.25f },
        { TileType.Town, 1.0f }
    };

    /// <summary>
    /// Determines whether a tile of the specified type is passable.
    /// </summary>
    /// <param name="type">The type of the tile to check.</param>
    /// <returns>True if the tile is passable; otherwise, false.</returns>
    public static bool IsPassable(TileType type) => 
        PassableLookup.TryGetValue(type, out bool passable) && passable;

     /// <summary>
    /// Retrieves the movement cost for a tile of the specified type.
    /// </summary>
    /// <param name="type">The type of the tile to check.</param>
    /// <returns>The movement cost of the tile. Defaults to 1.0 if the tile type is not found.</returns>
    public static float GetMovementCost(TileType type) => 
        MovementCostLookup.TryGetValue(type, out float cost) ? cost : 1.0f;
}