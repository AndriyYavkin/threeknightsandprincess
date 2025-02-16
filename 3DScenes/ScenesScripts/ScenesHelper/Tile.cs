using System.Collections.Generic;
using Godot;

namespace ScenesHelper;

public class Tile
{
    public TileType Type { get; set; }
    public bool IsPassable { get; set; }
    public Color TileColor { get; set; }
    public Node3D Object { get; set; }

    public Tile(TileType type)
    {
        Type = type;
        IsPassable = type != TileType.Water && type != TileType.Mountain; 

        // Set tile color based on type
        switch (type)
        {
            case TileType.Grass:
                TileColor = new Color(0, 1, 0); // Green
                break;
            case TileType.Water:
                TileColor = new Color(0, 0, 1); // Blue
                break;
            case TileType.Mountain:
                TileColor = new Color(0.5f, 0.5f, 0.5f); // Gray
                break;
            case TileType.Forest:
                TileColor = new Color(0, 0.5f, 0); // Dark Green
                break;
            case TileType.Town:
                TileColor = new Color(1, 0.5f, 0); // Orange
                break;
        }
    }
}