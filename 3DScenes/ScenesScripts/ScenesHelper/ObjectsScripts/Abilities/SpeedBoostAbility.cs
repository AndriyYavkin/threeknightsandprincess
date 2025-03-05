using Game.HelperCharacters;
using Godot;

namespace Game.ScenesHelper.ObjectsHelper.Abilities;

/// <summary>
/// Increases the character's speed.
/// </summary>
public class SpeedBoostAbility : AbilityModel
{
    private readonly float _speedMultiplier;
    private float _originalSpeed;

    public SpeedBoostAbility(float speedMultiplier)
    {
        _speedMultiplier = speedMultiplier;
    }

    public override string Description => $"Increases speed by {_speedMultiplier}x.";

    public override void Apply(CharacterBody3D character)
    {
        if (character is MainCharacterTemplate mainCharacter)
        {
            _originalSpeed = mainCharacter.Speed; // Store the original speed
            mainCharacter.Speed *= _speedMultiplier;
            GD.Print($"Speed increased to {mainCharacter.Speed}.");
        }
    }

    public override void Remove(CharacterBody3D character)
    {
        if (character is MainCharacterTemplate mainCharacter)
        {
            mainCharacter.Speed = _originalSpeed; // Restore the original speed
            GD.Print($"Speed restored to {mainCharacter.Speed}.");
        }
    }
}