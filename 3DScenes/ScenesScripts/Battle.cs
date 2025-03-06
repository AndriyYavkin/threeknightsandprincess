using Godot;
using System;

namespace Game.Scenes;

/// <summary>
/// Represents a battle scene with a grid-based system for managing tiles and entities.
/// </summary>
public partial class Battle : Node3D
{
	private const int GridSize = 8;
    private const float TileSize = 100.0f; // Adjust based on your floor scale

    public override void _Ready()
    {
        CreateGrid();
    }

    /// <summary>
    /// Creates a grid for the battle scene using a MultiMeshInstance3D.
    /// </summary>
    private void CreateGrid()
    {
        MultiMeshInstance3D multiMeshInstance = new MultiMeshInstance3D();
        MultiMesh multiMesh = new MultiMesh();

        multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        multiMesh.Mesh = new QuadMesh { Size = new Vector2(TileSize, TileSize) };
        multiMesh.InstanceCount = GridSize * GridSize;

        multiMeshInstance.Multimesh = multiMesh;
        AddChild(multiMeshInstance);

        // Set positions for each tile
        int index = 0;
        for (int x = 0; x < GridSize; x++)
        {
            for (int z = 0; z < GridSize; z++)
            {
                Transform3D transform = Transform3D.Identity;
                transform.Origin = new Vector3(x * TileSize, 0.01f, z * TileSize); // Slightly above ground

                multiMesh.SetInstanceTransform(index, transform);
                index++;
            }
        }

        // Optional: Add a transparent material
        StandardMaterial3D material = new StandardMaterial3D
        {
            AlbedoColor = new Color(1, 1, 1, 0.3f), // White with transparency
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha
        };
        multiMesh.Mesh.SurfaceSetMaterial(0, material);
    }
}
