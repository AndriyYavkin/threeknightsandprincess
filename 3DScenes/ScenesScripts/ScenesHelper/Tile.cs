using System.Collections.Generic;
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
        IsPassable = TileProperties.IsPassable(type);
    }

    public void SetPassable(bool isPassable) => IsPassable = isPassable;
    public float GetMovementCost() => TileProperties.GetMovementCost(Type);
}

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

    public static bool IsPassable(TileType type) => 
        PassableLookup.TryGetValue(type, out bool passable) ? passable : false;

    public static float GetMovementCost(TileType type) => 
        MovementCostLookup.TryGetValue(type, out float cost) ? cost : 1.0f;
}