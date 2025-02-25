using Godot;

namespace Characters.EntityModularNodesScripts;

/// <summary>
/// Represents an entity that can be interacted with in the game.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// The name of the entity.
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// The type of the entity (e.g., Ally, Neutral, Enemy).
    /// </summary>
    public EntitiesTypes Type { get; set; }

    /// <summary>
    /// Interacts with the entity.
    /// </summary>
    /// <param name="character">The character interacting with the entity.</param>
    void Interact(CharacterBody3D character);

    /// <summary>
    /// Initializes the entity, setting up its properties and modules.
    /// </summary>
    void Initialize();
}