using Godot;

namespace Game.ScenesHelper.ObjectsHelper.Abilities;

/// <summary>
/// Represents an ability that can be applied to a character.
/// </summary>
public interface IAbility
{
    /// <summary>
    /// Applies the ability to the character.
    /// </summary>
    /// <param name="character">The character to apply the ability to.</param>
    void Apply(CharacterBody3D character);

    /// <summary>
    /// Removes the ability from the character.
    /// </summary>
    /// <param name="character">The character to remove the ability from.</param>
    void Remove(CharacterBody3D character);

    /// <summary>
    /// Gets the description of the ability.
    /// </summary>
    string Description { get; }
}