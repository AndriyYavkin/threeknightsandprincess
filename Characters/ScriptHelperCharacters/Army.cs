using System.Collections.Generic;
using Game.HelperCharacters.Units;
using Godot;

namespace Game.HelperCharacters;

/// <summary>
/// Represents an army consisting of units.
/// </summary>
public class Army
{
    /// <summary>
    /// The list of units in the army.
    /// </summary>
    public List<Unit> Units { get; set; } = new();

    /// <summary>
    /// The maximum capacity of the army.
    /// </summary>
    public int MaxCapacity { get; set; } = 100;

    /// <summary>
    /// Adds a unit to the army.
    /// </summary>
    /// <param name="unit">The unit to add.</param>
    public void AddUnit(Unit unit)
    {
        if (Units.Count < MaxCapacity)
        {
            Units.Add(unit);
            GD.Print($"Added {unit.Name} to the army.");
        }
        else
        {
            GD.PrintErr("Army is at full capacity!");
        }
    }

    /// <summary>
    /// Removes a unit from the army.
    /// </summary>
    /// <param name="unit">The unit to remove.</param>
    public void RemoveUnit(Unit unit)
    {
        if (Units.Contains(unit))
        {
            Units.Remove(unit);
            GD.Print($"Removed {unit.Name} from the army.");
        }
        else
        {
            GD.PrintErr($"Unit {unit.Name} not found in the army!");
        }
    }
}