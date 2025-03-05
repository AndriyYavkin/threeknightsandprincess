using Godot;    
using Game.ScenesHelper.ObjectsHelper;

namespace Game.Objects;

/// <summary>
/// Represents a gold resource that can be picked up and used by a character.
/// </summary>
public partial class ResourceGold : ResourceModel
{
    public override string ItemName => "Gold";
    public override string Description => "What gold does";
    public override int Quantity => GD.RandRange(1, 10);

    public override void PickUp(CharacterBody3D character)
    {
        GD.Print($"{ItemName} picked up by {character.Name}.");
    }

    public override string ToString()
    {
        return $"ItemName: {ItemName}, Description: {Description}";
    }
}