using System.Collections.Generic;
using Godot;
using Game.ScenesHelper.ObjectsHelper.Abilities;

namespace Game.ScenesHelper.ObjectsHelper;

/// <summary>
/// Represents an artifact that can be picked up by a character.
/// </summary>
public abstract class ArtifactModel : IItem
{
    /// <summary>
    /// The name of the artifact.
    /// </summary>
    public abstract string ItemName { get; }

    /// <summary>
    /// The description of the artifact.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Gets an ability of the artifact
    /// </summary>
    public List<IAbility> Abilities { get; protected set; } = new();

    /// <summary>
    /// Indicates whether the artifact can be picked up.
    /// </summary>
    public bool IsPickable { get; protected set; } = true;

    /// <summary>
    /// Called when the artifact is picked up by a character.
    /// </summary>
    /// <param name="character">The character that picked up the artifact.</param>
    public void PickUp(CharacterBody3D character)
    {
        foreach (var ability in Abilities)
        {
            ability.Apply(character);
            GD.Print($"Applied ability: {ability.Description}");
        }
    }

    /// <summary>
    /// Called when the artifact is removed from the character's inventory.
    /// </summary>
    /// <param name="character">The character that removed the artifact.</param>
    public void OnRemove(CharacterBody3D character)
    {
        GD.Print($"{ItemName} removed from {character.Name}.");
        foreach (var ability in Abilities)
        {
            ability.Remove(character);
            GD.Print($"Removed ability: {ability.Description}");
        }
    }
}
