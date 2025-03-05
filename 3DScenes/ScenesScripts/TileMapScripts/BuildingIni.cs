using System;
using Godot;

namespace Game.ScenesHelper.TileMapScripts;

/// <summary>
/// Manages the placement of objects on the tile map.
/// </summary>
public partial class BuildingIni : Node3D , IMapInitializable
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

        AssignBuildingsToTiles();
    }

    /// <summary>
    /// Assigns all child objects to their corresponding tiles on the map.
    /// </summary>
    private void AssignBuildingsToTiles()
    {
        foreach (Node child in GetChildren())
        {
            if (child is Node3D node3D)
            {
                AssignBuildingsToTile(node3D);
            }
        }
    }

    /// <summary>
    /// Assigns a specific object to a tile on the map.
    /// </summary>
    /// <param name="building">The object to assign to a tile.</param>
    private void AssignBuildingsToTile(Node3D building)
    {
        int gridX = (int)Math.Round(building.Position.X / GridPositionConverter);
        int gridZ = (int)Math.Round(building.Position.Z / GridPositionConverter);

        if (gridX < 0 || gridX >= MapWidth || gridZ < 0 || gridZ >= MapHeight)
        {
            GD.PrintErr($"Building {building.Name} is outside the map boundaries! Object was deleted");
            building.QueueFree();
            return;
        }

        var tile = Scenes.TileMap.Map[gridX, gridZ];
        if (tile.ContainsObject != null)
        {
            GD.PrintErr($"Building {building.Name} can't be assigned! Tile is already taken! Object was deleted");
            building.QueueFree();
            return;
        }

        tile.ContainsObject = building;
        tile.IsEntity = false;
        building.Position = new Vector3(gridX * GridPositionConverter, 0, gridZ * GridPositionConverter);
    }
}