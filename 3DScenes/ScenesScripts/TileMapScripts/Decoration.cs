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
                GD.Print($"Processed decoration {decorationNode.Name}");
            }
        }
    }

    private void ProcessDecoration(MeshInstance3D decorationNode)
    {
        // Dynamically calculate the size of the decoration
        Vector3 size = CalculateSize(decorationNode);

        // Mark tiles as non-passable if the decoration blocks movement
        MarkTilesAsNonPassable(decorationNode, size);
    }

    private Vector3 CalculateSize(Node3D decorationNode)
    {
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
        GD.Print($"Using default size for {decorationNode.Name}");
        return new Vector3(1, 1, 1) * decorationNode.Scale; // Apply scaling to default size
    }

    private void MarkTilesAsNonPassable(Node3D decorationNode, Vector3 size)
    {
        // Calculate the grid position of the decoration's center
        int centerX = (int)Math.Round(decorationNode.Position.X / GridPositionConverter);
        int centerZ = (int)Math.Round(decorationNode.Position.Z / GridPositionConverter);

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
                    var tile = Scenes.TileMap.map[x, z];
                    if(tile.IsPassable)
                    {
                        tile.IsPassable = false;
                        //VisualizeTile(x,z);
                        GD.Print($"Marked tile at ({x}, {z}) as non-passable due to decoration {decorationNode.Name}");
                    }
                    GD.Print($"Tile at ({x}, {z}) was already not passable!");
                }
            }
        }
    }

    private void VisualizeTile(int x, int z)
    {
        var tile = Scenes.TileMap.map[x, z];

        // Create a visual representation for the tile
        var meshInstance = new MeshInstance3D();
        meshInstance.Mesh = new BoxMesh();
        meshInstance.MaterialOverride = new StandardMaterial3D
        {
            AlbedoColor = GetTileColor(tile)
        };

     // Position the tile
        meshInstance.Position = new Vector3(x * GridPositionConverter, 1, z * GridPositionConverter);
        AddChild(meshInstance);
    }


    private Color GetTileColor(Tile tile)
    {
        // If the tile is non-passable, color it purple
        if (!tile.IsPassable)
        {
            return new Color(0.5f, 0, 0.5f); // Purple
        }

        // Otherwise, use the tile's default color
        return tile.TileColor;
    }
}