using Godot;

namespace ScenesHelper.ObjectsHelper;

/// <summary>
/// Represents a resource that can be picked up by a character.
/// </summary>
public abstract class ResourceModel : IItem
{
    /// <summary>
    /// The name of the resource.
    /// </summary>
    public abstract string ItemName { get; }

    /// <summary>
    /// The description of the resource.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Indicates whether the resource can be picked up.
    /// </summary>
    public bool IsPickable { get; protected set; } = true;

    /// <summary>
    /// Called when the resource is picked up by a character.
    /// </summary>
    /// <param name="character">The character that picked up the resource.</param>
    public abstract void PickUp(CharacterBody3D character);
}