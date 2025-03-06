using System.Collections.Generic;
using System.Linq;
using Game.HelperCharacters.Units;
using Godot;

namespace Game.HelperCharacters;

/// <summary>
/// Represents an army consisting of units.
/// </summary>
public class Army
{
    /// <summary>
    /// The list of unit stacks in the army.
    /// </summary>
    public List<Unit> Units { get; set; } = new();

    /// <summary>
    /// The maximum number of unit slots in the army.
    /// </summary>
    public int MaxCapacity { get; set; } = 8;

    /// <summary>
    /// Adds a unit to the army.
    /// </summary>
    /// <param name="unit">The unit to add.</param>
    /// <param name="count">The number of units to add.</param>
    public void AddUnit(Unit unit, int count = 1)
    {
         // Check if the army has reached its maximum capacity
        if (Units.Count >= MaxCapacity)
        {
            GD.PrintErr("Army is at full capacity!");
            return;
        }

        // Check if there's already a stack of this unit type
        var existingStack = Units.FirstOrDefault(u => u.GetType() == unit.GetType());
        if (existingStack != null)
        {
            // Add to the existing stack
            existingStack.Count += count;
            GD.Print($"Added {count} {unit.Name} to the existing stack. Total: {existingStack.Count}");
        }
        else
        {
            // Create a new stack
            unit.Count = count;
            Units.Add(unit);
            GD.Print($"Added {count} {unit.Name} to the army.");
        }
    }

    /// <summary>
    /// Removes a unit from the army.
    /// </summary>
    /// <param name="unit">The unit to remove.</param>
    /// <param name="count">The number of units to remove.</param>
    public void RemoveUnit(Unit unit, int count = 1)
    {
        var existingStack = Units.FirstOrDefault(u => u.GetType() == unit.GetType());
        if (existingStack != null)
        {
            if (existingStack.Count > count)
            {
                // Reduce the stack size
                existingStack.Count -= count;
                GD.Print($"Removed {count} {unit.Name} from the stack. Remaining: {existingStack.Count}");
            }
            else
            {
                // Remove the entire stack
                Units.Remove(existingStack);
                GD.Print($"Removed all {unit.Name} from the army.");
            }
        }
        else
        {
            GD.PrintErr($"Unit {unit.Name} not found in the army!");
        }
    }

    /// <summary>
    /// Gets the total number of units in the army.
    /// </summary>
    public int GetTotalUnitCount()
    {
        return Units.Sum(u => u.Count);
    }
}