using System;
using Godot;

namespace Game.HelperCharacters.Units;

/// <summary>
/// Represents the battle-specific stats of a character, entity, or creature.
/// </summary>
public class BattleStats
{
    /// <summary>
    /// The current health of the unit.
    /// </summary>
    public int Health { get; set; }

    /// <summary>
    /// The attack power of the unit.
    /// </summary>
    public int Attack { get; set; }

    /// <summary>
    /// The base damage range of the unit.
    /// </summary>
    public (int Min, int Max) Damage { get; set; }

    /// <summary>
    /// The defense power of the unit.
    /// </summary>
    public int Defense { get; set; }

    /// <summary>
    /// The speed of the unit.
    /// </summary>
    public int Speed { get; set; }

    public const float SpeedOnMap = 5f;

    /// <summary>
    /// Initializes the battle stats with default values.
    /// </summary>
    /// <param name="health">Health of the unit.</param>
    /// <param name="attack">Attack of the unit.</param>
    /// <param name="damage">Damage of the unit.</param>
    /// <param name="defense">Defense of the unit.</param>
    /// <param name="speed">Speed of the unit.</param>
    public BattleStats(int health, int attack, (int Min, int Max) damage, int defense, int speed)
    {
        Health = health;
        Attack = attack;
        Damage = damage;
        Defense = defense;
        Speed = speed;
    }

        /// <summary>
    /// Calculates the damage dealt by the unit.
    /// </summary>
    /// <returns>The calculated damage.</returns>
    public int CalculateDamage()
    {
        // Calculate the base damage (random between Min and Max)
        int baseDamage = new Random().Next(Damage.Min, Damage.Max + 1);

        // Add 4% of Attack as a modifier
        float attackModifier = Attack * 0.04f;
        int totalDamage = baseDamage + (int)attackModifier;

        GD.Print($"Base Damage: {baseDamage}, Attack Modifier: {attackModifier}, Total Damage: {totalDamage}");
        return totalDamage;
    }
}