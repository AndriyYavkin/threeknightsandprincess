using System;
using Godot;

namespace ScenesHelper.TileMapScripts;

/// <summary>
/// Manages the placement of objects on the tile map.
/// </summary>
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

    /// <summary>
    /// Assigns all child objects to their corresponding tiles on the map.
    /// </summary>
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

    /// <summary>
    /// Assigns a specific object to a tile on the map.
    /// </summary>
    /// <param name="obj">The object to assign to a tile.</param>
    private void AssignObjectToTile(Node3D obj)
    {
        int gridX = (int)Math.Round(obj.Position.X / GridPositionConverter);
        int gridZ = (int)Math.Round(obj.Position.Z / GridPositionConverter);

        if (gridX < 0 || gridX >= MapWidth || gridZ < 0 || gridZ >= MapHeight)
        {
            GD.PrintErr($"Object {obj.Name} is outside the map boundaries! Object was deleted");
            obj.QueueFree();
            return;
        }

        var tile = Scenes.TileMap.Map[gridX, gridZ];
        if (tile.ContainsObject != null)
        {
            GD.PrintErr($"Object {obj.Name} can't be assigned! Tile is already taken! Object was deleted");
            obj.QueueFree();
            return;
        }

        tile.ContainsObject = obj;
        tile.IsEntity = false;
        obj.Position = new Vector3(gridX * GridPositionConverter, 0, gridZ * GridPositionConverter);
    }
}