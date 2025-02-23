using System.Collections.Generic;
using Godot;

namespace GameHelperCharacters;

public class Inventory
{
    public List<IItem> Items { get; private set; } = new List<IItem>();

    // Add an item to the inventory
    public void AddItem(IItem item)
    {
        Items.Add(item);
        GD.Print($"Added {item.Name} to inventory.");
    }

    // Remove an item from the inventory
    public void RemoveItem(IItem item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
            GD.Print($"Removed {item.Name} from inventory.");
        }
        else
        {
            GD.PrintErr($"Item {item.Name} not found in inventory.");
        }
    }

    // Use an item from the inventory
    public void UseItem(IItem item, CharacterBody3D character)
    {
        if (Items.Contains(item))
        {
            item.PickUp(character);
            RemoveItem(item);
        }
        else
        {
            GD.PrintErr($"Item {item.Name} not found in inventory.");
        }
    }

    public void DebugPrint()
    {
        foreach(var item in Items)
        {
            GD.Print(item.ToString());
        }        
    }
}