using System;
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

        // Initialize all child decorations
        InitializeChildDecorations();
    }

    private void InitializeChildDecorations()
    {
        foreach (Node child in GetChildren())
        {
            if (child is MeshInstance3D decorationNode)
            {
                // Treat the child as a decoration
                ProcessDecoration(decorationNode);
            }
        }
    }

    private void ProcessDecoration(MeshInstance3D decorationNode)
    {
        bool blocksMovement = (bool)decorationNode.GetMeta("BlocksMovement", false);

        if (blocksMovement)
        {
            // Dynamically calculate the size of the decoration (accounting for scaling)
            Vector3 size = TileMapHelper.CalculateSize(decorationNode);

            // Mark tiles as non-passable
            TileMapHelper.MarkTilesAsNonPassable(decorationNode, size);
        }
        else
        {
            GD.Print($"Decoration {decorationNode.Name} does not block movement.");
        }
    }

    /// <summary>
    /// Debug method to visualize what tiles were blocked via decorations
    /// </summary>
    private void VisualizeTile(int x, int z)
    {
        var tile = Scenes.TileMap.Map[x, z];

        // Create a visual representation for the tile
        var meshInstance = new MeshInstance3D();
        meshInstance.Mesh = new BoxMesh();
     // Position the tile
        meshInstance.Position = new Vector3(x * GridPositionConverter, 1, z * GridPositionConverter);
        AddChild(meshInstance);
    }
}