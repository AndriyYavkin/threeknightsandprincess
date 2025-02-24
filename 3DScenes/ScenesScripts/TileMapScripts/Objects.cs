using System;
using GameHelperCharacters;
using Godot;

namespace ScenesHelper.TileMapScripts;


public partial class Objects : Node3D , IMapInitializable
{
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public float GridPositionConverter { get; set; }

    public void Initialize(int mapWidth, int mapHeight, float gridPositionConverter)
    {
        
        if(Scenes.TileMap.Map is null)
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
        if(Scenes.TileMap.Map[gridX,gridZ].Object is null)
        {
            if (gridX >= 0 && gridX < MapWidth && gridZ >= 0 && gridZ < MapHeight)
            {
                var tile = Scenes.TileMap.Map[gridX, gridZ];
                tile.Object = obj;
                tile.IsEntity = false;
                obj.Position = new Vector3(gridX * GridPositionConverter, 0 , gridZ * GridPositionConverter);
            }
            else
            {
                GD.PrintErr($"Object {obj.Name} is outside the map boundaries! Object was deleted");
                obj.QueueFree();
            }
        }
        else
        {
            GD.PrintErr($"Object {obj.Name} can't be assigned! tile is already taken! Object was deleted");
            obj.QueueFree();
        }
    }
}