using Game.HelperCharacters.Units;

namespace Game.HelperCharacters.Units;

/// <summary>
/// Represents a peasant unit.
/// </summary>
public class Peasant : Unit
{
    public Peasant() : base("Peasant", new BattleStats(20, 5, (1, 2), 2, 4))
    {
    }
}

/// <summary>
/// Represents an archer unit.
/// </summary>
public class Archer : Unit
{
    public Archer() : base("Archer", new BattleStats(15, 8, (3, 5), 3, 5))
    {
    }
}

/// <summary>
/// Represents a knight unit.
/// </summary>
public class Knight : Unit
{
    public Knight() : base("Knight", new BattleStats(50, 15, (5, 6), 10, 3))
    {
    }
}