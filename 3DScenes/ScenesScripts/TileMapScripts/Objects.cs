using System;
using Godot;

namespace ScenesHelper.TileMapScripts;


public partial class Objects : Node3D , IMapInitializable
{
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public float GridPositionConverter { get; set; }

    public void Initialize(int mapWidth, int mapHeight, float gridPositionConverter)
    {
        
        if(Scenes.TileMap.map is null)
        {
            GD.PrintErr("Tile map is null! Check your order of initialization! Objects won't initialize and work!");
            return;
        }        
        
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        GridPositionConverter = gridPositionConverter;

        AssignObjectsToTiles();
    }


    private void AssignObjectsToTiles()
    {
        foreach (Node child in GetChildren())
        {
            if (child is MeshInstance3D node3D)
            {
                AssignObjectToTile(node3D);
            }
        }
    }

    private void AssignObjectToTile(Node3D obj)
    {
        int gridX = (int)Math.Round(obj.Position.X / GridPositionConverter);
        int gridZ = (int)Math.Round(obj.Position.Z / GridPositionConverter);

        if (gridX >= 0 && gridX < MapWidth && gridZ >= 0 && gridZ < MapHeight)
        {
            var tile = Scenes.TileMap.map[gridX, gridZ];
            tile.Object = obj;
			obj.Position = new Vector3(gridX * 2, 0 , gridZ * 2);
            GD.Print($"Assigned object {obj.Name} to tile at ({gridX}, {gridZ})");
        }
        else
        {
            GD.PrintErr($"Object {obj.Name} is outside the map boundaries!");
        }
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