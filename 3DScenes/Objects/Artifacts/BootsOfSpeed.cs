using Game.ScenesHelper.ObjectsHelper;
using Game.ScenesHelper.ObjectsHelper.Abilities;

namespace Game.Objects;

/// <summary>
/// Represents a first artifact that can be picked up and used by a character.
/// </summary>
public partial class BootsOfSpeed : ArtifactModel
{
    public override string ItemName => "First testing artifact";
    public override string Description => "Does something";
    public BootsOfSpeed()
    {
        // Add a speed boost ability to this artifact
        Abilities.Add(new SpeedBoostAbility(1.5f));
    }

    public override string ToString()
    {
        return $"ItemName: {ItemName}, Description: {Description}";
    }
}

