using System;
using Godot;

namespace Game.ScenesHelper.TileMapScripts;

/// <summary>
/// Provides helper methods for tile map operations, such as calculating object sizes and marking tiles as non-passable.
/// </summary>
public static class TileMapHelper
{
    /// <summary>
    /// Gets or sets the width of the map in tiles.
    /// </summary>
    public static int MapWidth { get; set; }

    /// <summary>
    /// Gets or sets the height of the map in tiles.
    /// </summary>
    public static int MapHeight { get; set; }

    /// <summary>
    /// Gets or sets the size of each grid cell in world units.
    /// </summary>
    public static float GridPositionConverter { get; set;} 


    /// <summary>
    /// Calculates the scaled size of a decoration or object based on its mesh.
    /// </summary>
    /// <param name="decorationNode">The node representing the decoration or object.</param>
    /// <returns>The scaled size of the node's mesh.</returns>
    public static Vector3 CalculateSize(Node3D decorationNode)
    {
        if (decorationNode == null)
        {
            GD.PrintErr("Decoration node is null.");
            return Vector3.Zero;
        }

        // Try to get the size from a MeshInstance3D
        var meshInstance = decorationNode.GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
        if (meshInstance != null && meshInstance.Mesh != null)
        {
            // Get the AABB (Axis-Aligned Bounding Box) of the mesh
            var aabb = meshInstance.Mesh.GetAabb();

            // Apply the node's scale to the AABB size
            Vector3 scaledSize = new Vector3(
                aabb.Size.X * decorationNode.Scale.X,
                aabb.Size.Y * decorationNode.Scale.Y,
                aabb.Size.Z * decorationNode.Scale.Z
            );

            GD.Print($"Calculated scaled size from MeshInstance3D AABB: {scaledSize} for {decorationNode.Name}");
            return scaledSize;
        }

        // Default size if no MeshInstance3D is found
        GD.Print($"Using default size for {decorationNode.Name}.");
        return new Vector3(1, 1, 1) * decorationNode.Scale; // Apply scaling to default size
    }

    /// <summary>
    /// Marks tiles as non-passable based on the position and size of a decoration or object.
    /// </summary>
    /// <param name="usedNode">The node representing the decoration or object.</param>
    /// <param name="size">The size of the decoration or object.</param>
    public static void MarkTilesAsNonPassable(Node3D usedNode, Vector3 size)
    {
        if (usedNode == null)
        {
            GD.PrintErr("Used node is null.");
            return;
        }

        if (size == Vector3.Zero)
        {
            GD.PrintErr("Size is zero.");
            return;
        }

        // Calculate the grid position of the decoration's center
        int centerX = (int)Math.Round(usedNode.Position.X / GridPositionConverter);
        int centerZ = (int)Math.Round(usedNode.Position.Z / GridPositionConverter);

        // Calculate the bounds of the decoration in grid coordinates
        int minX = Math.Max(0, centerX - (int)Math.Floor(size.X / 2 / GridPositionConverter));
        int maxX = Math.Min(MapWidth - 1, centerX + (int)Math.Floor(size.X / 2 / GridPositionConverter));
        int minZ = Math.Max(0, centerZ - (int)Math.Floor(size.Z / 2 / GridPositionConverter));
        int maxZ = Math.Min(MapHeight - 1, centerZ + (int)Math.Floor(size.Z / 2 / GridPositionConverter));

        // Mark all tiles within the bounds as non-passable
        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                if (x >= 0 && x < MapWidth && z >= 0 && z < MapHeight)
                {
                    var tile = Scenes.TileMap.Map[x, z];
                    if (tile.GetPassable())
                    {
                        tile.SetPassable(false);
                        GD.Print($"Marked tile at ({x}, {z}) as non-passable due to decoration or object {usedNode.Name}");
                    }
                    GD.Print($"Tile at ({x}, {z}) was already not passable!");
                }
            }
        }
    }

}