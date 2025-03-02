using Godot;
using System;

namespace ScenesHelper.TileMapScripts;

/// <summary>
/// Manages the placement and interaction of entities on the tile map.
/// </summary>
public partial class Entities : Node3D, IMapInitializable
{
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }
    public float GridPositionConverter { get; set; }

    private const float EntityYPosition = 1.0f;

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

    /// <summary>
    /// Initializes all child entities and assigns them to tiles on the map.
    /// </summary>
    private void InitializeEntities()
    {
        foreach (Node child in GetChildren())
        {
            if (child is Node3D entityNode)
            {
                // Assign entity to tile
                AssignEntityToTile(entityNode);
                //Initialize interacts with entities
                if(child is Characters.EntityModularNodesScripts.IInteractableEntity initializableCharacter)
                {
                    initializableCharacter.Initialize();
                }
            }
        }
    }

    /// <summary>
    /// Assigns a specific entity to a tile on the map.
    /// </summary>
    /// <param name="entityNode">The entity to assign to a tile.</param>
    private void AssignEntityToTile(Node3D entityNode)
    {
        int gridX = (int)Math.Round(entityNode.Position.X / GridPositionConverter);
        int gridZ = (int)Math.Round(entityNode.Position.Z / GridPositionConverter);

        if (gridX < 0 || gridX >= MapWidth || gridZ < 0 || gridZ >= MapHeight)
        {
            GD.PrintErr($"Entity {entityNode.Name} is outside the map boundaries!");
            return;
        }

        var tile = Scenes.TileMap.Map[gridX, gridZ];
        tile.ContainsObject = entityNode;
        tile.IsEntity = true;
        entityNode.Position = new Vector3(gridX * GridPositionConverter, EntityYPosition, gridZ * GridPositionConverter);
    }

    /// <summary>
    /// Handles entity interaction when the player clicks on an entity.
    /// </summary>
    /// <param name="characterPosition">The position of the character interacting with the entity.</param>
    public void HandleEntityInteraction(Vector3 characterPosition)
    {
        int gridX = (int)Math.Round(characterPosition.X / GridPositionConverter);
        int gridZ = (int)Math.Round(characterPosition.Z / GridPositionConverter);

        if (gridX >= 0 && gridX < MapWidth && gridZ >= 0 && gridZ < MapHeight)
        {
            var tile = Scenes.TileMap.Map[gridX, gridZ];
            if (tile.ContainsObject != null && tile.IsEntity)
            {
                //entity.Interact(CharacterHelper.Character); // Not implemented yet
            }
        }
    }
}