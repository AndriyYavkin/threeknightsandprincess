using Godot;
using ScenesHelper.TileMapScripts;

namespace ScenesHelper.ObjectsHelper;

/// <summary>
/// Represents an artifact that can be picked up by a character.
/// </summary>
public abstract class ArtifactModel : IItem
{
    /// <summary>
    /// The name of the artifact.
    /// </summary>
    public abstract string ItemName { get; }

    /// <summary>
    /// The description of the artifact.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Indicates whether the artifact can be picked up.
    /// </summary>
    public bool IsPickable { get; protected set; } = true;

    /// <summary>
    /// Called when the artifact is picked up by a character.
    /// </summary>
    /// <param name="character">The character that picked up the artifact.</param>
    public abstract void PickUp(CharacterBody3D character);

    /// <summary>
    /// Called when the artifact is used by a character.
    /// </summary>
    /// <param name="character">The character that used the artifact.</param>
    public abstract void OnUse(CharacterBody3D character);
}
