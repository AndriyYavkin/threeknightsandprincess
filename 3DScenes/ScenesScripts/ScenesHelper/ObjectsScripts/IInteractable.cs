using Godot;

namespace Game.ScenesHelper.ObjectsHelper;

/// <summary>
/// Defines an interface for objects that can be interacted with.
/// </summary>
public interface IInteractableObject
{
    /// <summary>
    /// Called when the object is interacted with.
    /// </summary>
    /// <param name="character">The character interacting with the object.</param>
    void OnInteract(CharacterBody3D character);
}