using Godot;
using UI;

namespace GameHelperCharacters;

/// <summary>
/// Defines the common properties and methods for all characters in the game.
/// </summary>
public interface ICharacterTemplate : IUIDisplayable
{
    /// <summary>
    /// Gets or sets character's name
    /// </summary>
    public string CharacterName { get; set; }

    /// <summary>
    /// Gets or sets the character's inventory.
    /// </summary>
    public Inventory Inventory { get; set; }

    /// <summary>
    /// Gets or sets the current grid position of the character.
    /// </summary>
    public Vector3I GridPosition { get; set; }
}