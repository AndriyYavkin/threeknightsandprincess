using Godot;
using UI;

namespace ScenesHelper.ObjectsHelper;

/// <summary>
/// Represents an object that can be picked up by a character.
/// </summary>
public interface IObjectPickable : IUIDisplayable
{
    /// <summary>
    /// Called when the object is picked up by a character.
    /// </summary>
    /// <param name="character">The character that picked up the object.</param>
    void OnPickUp(CharacterBody3D character);

    /// <summary>
    /// The linked item that will be added to the inventory when picked up.
    /// </summary>
    public IItem LinkedItem { get; set; }
}
