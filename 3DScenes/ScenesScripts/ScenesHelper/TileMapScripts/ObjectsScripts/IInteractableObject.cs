using Godot;

namespace ScenesHelper.ObjectsHelper;

/// <summary>
/// Defines an interface for objects that can be interacted with.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the object is interacted with.
    /// </summary>
    /// <param name="character">The character interacting with the object.</param>
    void OnInteract(CharacterBody3D character);
}