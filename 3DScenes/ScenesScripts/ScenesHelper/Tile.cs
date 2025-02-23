using Godot;

namespace ScenesHelper;

public class Tile
{
    public TileType Type { get; set; }
    public bool IsPassable { get; set; }
    public Node3D Object { get; set; } = null; //It can be both just a resource or entity
    public MeshInstance3D TileMesh { get; set; } = null;
    public bool IsEntity { get; set; } = false;

    public Tile(TileType type, Texture2D texture = null)
    {   
        Type = type;
        IsPassable = type != TileType.Water && type != TileType.Mountain && type != TileType.NotDefined; 

        TileMesh = new MeshInstance3D();

        if (type == TileType.NotDefined)
        {
            // Use a QuadMesh for undefined tiles
            TileMesh.Mesh = new QuadMesh();
            TileMesh.RotateX(Mathf.DegToRad(-90)); // Rotate to lie flat on the ground

            // Apply a default material (optional)
            var material = new StandardMaterial3D();
            material.AlbedoColor = new Color(0.5f, 0.5f, 0.5f); // Gray color for undefined tiles
            TileMesh.MaterialOverride = material;
        }
    }
}