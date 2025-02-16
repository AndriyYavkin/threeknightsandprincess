using Godot;

namespace ScenesHelper.TileMapScripts;

public partial class Decoration : Node3D, IMapInitializable
{
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public float GridPositionConverter { get; set; }

    public void Initialize(int mapWidth, int mapHeight, float gridPositionConverter)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        GridPositionConverter = gridPositionConverter;
    }

    // Check if a point is inside the decoration's bounds
    /*public bool ContainsPoint(Vector3 point)
    {
        var min = Position - Size / 2;
        var max = Position + Size / 2;
        return point.X >= min.X && point.X <= max.X &&
               point.Z >= min.Z && point.Z <= max.Z;
    }*/
}