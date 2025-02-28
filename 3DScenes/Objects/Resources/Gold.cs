using Godot;    
using ScenesHelper.ObjectsHelper;

namespace Objects;

/// <summary>
/// Represents a first artifact that can be picked up and used by a character.
/// </summary>
public partial class ResourceGold : ResourceModel
{
    public override string ItemName => "Gold";
    public override string Description => "What gold does";

    public override void PickUp(CharacterBody3D character)
    {
        GD.Print($"{ItemName} picked up by {character.Name}.");
    }

    public override string ToString()
    {
        return $"ItemName: {ItemName}, Description: {Description}";
    }
}