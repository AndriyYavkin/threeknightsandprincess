using Godot;

namespace GameHelperCharacters;

/// <summary>
/// Defines the common properties and methods for all characters in the game.
/// </summary>
public interface ICharacterTemplate
{
    /// <summary>
    /// The character's inventory.
    /// </summary>
    public Inventory Inventory { get; set; }

    /// <summary>
    /// The current grid position of the character.
    /// </summary>
    public Vector3I GridPosition { get; set; }
}