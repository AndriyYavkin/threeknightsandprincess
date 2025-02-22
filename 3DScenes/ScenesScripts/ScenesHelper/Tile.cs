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
        IsPassable = type != TileType.Water && type != TileType.Mountain; 

        TileMesh = new MeshInstance3D();
        TileMesh.Mesh = new QuadMesh(); // Default to a quad mesh
        TileMesh.RotateX(Mathf.DegToRad(-90));


        var material = new StandardMaterial3D();
        material.AlbedoTexture = texture; // Set the texture from the TileMap

        // Stretch the texture to fit the tile
        material.Uv1Scale = new Vector3(0.25f, 0.25f, 1); // Scale the texture to cover the entire tile
        TileMesh.MaterialOverride = material;

    }
}