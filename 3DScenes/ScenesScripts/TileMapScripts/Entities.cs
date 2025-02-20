using Godot;
using System;

namespace ScenesHelper.TileMapScripts;

public partial class Entities : Node3D, IMapInitializable
{
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public float GridPositionConverter { get; set; }

    public void Initialize(int mapWidth, int mapHeight, float gridPositionConverter)
    {
        if (Scenes.TileMap.Map is null)
        {
            GD.PrintErr("Tile map is null! Check your order of initialization! Entities won't initialize and work!");
            return;
        }

        MapWidth = mapWidth;
        MapHeight = mapHeight;
        GridPositionConverter = gridPositionConverter;

        InitializeEntities();
    }

    private void InitializeEntities()
    {
        foreach (Node child in GetChildren())
        {
            if (child is Node3D entityNode)
            {
                // Assign entity to tile
                AssignEntityToTile(entityNode);
                //Initialize interacts with entities
                if(child is Characters.EntityModularNodesScripts.IInteractable initializableCharacter)
                {
                    initializableCharacter.Initialize();
                }
            }
        }
    }

    private void AssignEntityToTile(Node3D entityNode)
    {
        int gridX = (int)Math.Round(entityNode.Position.X / GridPositionConverter);
        int gridZ = (int)Math.Round(entityNode.Position.Z / GridPositionConverter);

        if (gridX >= 0 && gridX < MapWidth && gridZ >= 0 && gridZ < MapHeight)
        {
            var tile = Scenes.TileMap.Map[gridX, gridZ];
            tile.Object = entityNode;
            tile.IsEntity = true;
            entityNode.Position = new Vector3(gridX * 2, 1 , gridZ * 2);
        }
        else
        {
            GD.PrintErr($"Entity {entityNode.Name} is outside the map boundaries!");
        }
    }

    // Handle entity interaction when the player clicks on an entity
    public void HandleEntityInteraction(Vector3 characterPosition)
    {
        int gridX = (int)Math.Round(characterPosition.X / GridPositionConverter);
        int gridZ = (int)Math.Round(characterPosition.Z / GridPositionConverter);

        if (gridX >= 0 && gridX < MapWidth && gridZ >= 0 && gridZ < MapHeight)
        {
            var tile = Scenes.TileMap.Map[gridX, gridZ];
            if (tile.Object != null && tile.IsEntity)
            {
                //entity.Interact(CharacterHelper.Character);
            }
        }
    }
}