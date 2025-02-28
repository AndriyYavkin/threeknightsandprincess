using System.Runtime.CompilerServices;
using Godot;

namespace ScenesHelper.ObjectsHelper;

/// <summary>
/// Defines an interface for items that can be picked up and used by characters.
/// </summary>
public interface IItem
{
    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    string ItemName { get; }

    /// <summary>
    /// Gets the description of the item.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Called when the item is picked up by a character.
    /// </summary>
    /// <param name="character">The character that picked up the item.</param>
    void PickUp(CharacterBody3D character);
}