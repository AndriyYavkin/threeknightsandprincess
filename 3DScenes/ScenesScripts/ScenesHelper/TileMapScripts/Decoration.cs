using Godot;

namespace ScenesHelper.TileMapScripts;

/// <summary>
/// Manages the placement and behavior of decorations on the tile map.
/// </summary>
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

    /// <summary>
    /// Initializes all child decorations and processes their properties.
    /// </summary>
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

    /// <summary>
    /// Processes a decoration node, checking if it blocks movement and marking tiles accordingly.
    /// </summary>
    /// <param name="decorationNode">The decoration node to process.</param>
    private static void ProcessDecoration(MeshInstance3D decorationNode)
    {
        if (!decorationNode.HasMeta("BlocksMovement"))
        {
            GD.Print($"Decoration {decorationNode.Name} does not have the 'BlocksMovement' meta. Assuming it does not block movement.");
            return;
        }

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
    /// Debug method to visualize what tiles were blocked via decorations.
    /// </summary>
    /// <param name="x">The x-coordinate of the tile in the grid.</param>
    /// <param name="z">The z-coordinate of the tile in the grid.</param>
    private void VisualizeTile(int x, int z)
    {
        // Create a visual representation for the tile
        var meshInstance = new MeshInstance3D();
        meshInstance.Mesh = new BoxMesh();
         // Position the tile
        meshInstance.Position = new Vector3(x * GridPositionConverter, 1, z * GridPositionConverter);
        AddChild(meshInstance);
    }
}