using Game.ScenesHelper.ObjectsHelper;
using Game.ScenesHelper.ObjectsHelper.Abilities;
using Godot;

namespace Game.Objects;

/// <summary>
/// Represents a first artifact that can be picked up and used by a character.
/// </summary>
public partial class BootsOfSpeed : ArtifactModel
{
    public override string ItemName => "Boots of Speed";
    public override string Description => "Does something";
    public BootsOfSpeed()
    {
        // Add a speed boost ability to this artifact
        Abilities.Add(new SpeedBoostAbility(1.25f));
    }

    public override string ToString()
    {
        return $"ItemName: {ItemName}, Description: {Description}";
    }
}

