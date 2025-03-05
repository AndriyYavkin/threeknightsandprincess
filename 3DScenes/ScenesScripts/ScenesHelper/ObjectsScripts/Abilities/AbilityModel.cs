using Godot;

namespace Game.ScenesHelper.ObjectsHelper.Abilities;

/// <summary>
/// Base class for abilities.
/// </summary>
public abstract class AbilityModel : IAbility
{
    public abstract string Description { get; }

    public abstract void Apply(CharacterBody3D character);
}