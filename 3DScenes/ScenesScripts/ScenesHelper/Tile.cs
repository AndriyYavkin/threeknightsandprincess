using Godot;

namespace ScenesHelper;

public class Tile
{
    public TileType Type { get; set; }
    public bool IsPassable { get; set; }

    public Tile(TileType type)
    {
        Type = type;
        IsPassable = type != TileType.Water && type != TileType.Mountain;
    }
}