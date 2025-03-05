using Godot;
using Game.UI;

namespace Game.HelperCharacters;

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

    /// <summary>
    /// Gets or sets the character's army.
    /// </summary>
    Army Army { get; set; }

    /// <summary>
    /// Called when the character is initialized.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Called when the character is updated (e.g., during battle or movement).
    /// </summary>
    void Update();
}