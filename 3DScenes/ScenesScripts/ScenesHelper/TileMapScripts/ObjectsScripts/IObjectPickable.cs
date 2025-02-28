using Godot;

namespace ScenesHelper.ObjectsHelper;

/// <summary>
/// Represents an object that can be picked up by a character.
/// </summary>
public interface IObjectPickable
{
    /// <summary>
    /// Called when the object is picked up by a character.
    /// </summary>
    /// <param name="character">The character that picked up the object.</param>
    void OnPickUp(CharacterBody3D character);

    public IItem LinkedItem { get; set; }
}
