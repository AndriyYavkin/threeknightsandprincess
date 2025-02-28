using GameHelperCharacters;
using Godot;
using ScenesHelper.ObjectsHelper;

namespace Objects;

/// <summary>
/// Represents a first artifact that can be picked up and used by a character.
/// </summary>
public partial class FirstArtifact : ArtifactModel
{
    public override string ItemName => "First testing artifact";
    public override string Description => "Does something";

    public override void PickUp(CharacterBody3D character)
    {
        GD.Print($"{ItemName} picked up by {character.Name}.");
    }

    public override void OnUse(CharacterBody3D character)
    {
        GD.Print($"{ItemName} used by {character.Name}.");
    }

    public override string ToString()
    {
        return $"ItemName: {ItemName}, Description: {Description}";
    }
}

