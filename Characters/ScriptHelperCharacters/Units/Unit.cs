namespace Game.HelperCharacters.Units;

/// <summary>
/// Represents a unit in an army.
/// </summary>
public class Unit
{
    /// <summary>
    /// The name of the unit.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The battle-specific stats of the unit.
    /// </summary>
    public BattleStats Stats { get; set; }

    /// <summary>
    /// Initializes a new unit with the specified name and stats.
    /// </summary>
    public Unit(string name, BattleStats stats)
    {
        Name = name;
        Stats = stats;
    }
}