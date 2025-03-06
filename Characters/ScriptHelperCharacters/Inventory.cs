using System;
using System.Collections.Generic;
using Godot;
using Game.ScenesHelper.ObjectsHelper;
using Game.ScenesHelper.ObjectsHelper.Abilities;

namespace Game.HelperCharacters;

/// <summary>
/// Represents a character's inventory, allowing the storage and management of items.
/// </summary>
public class Inventory
{
    /// <summary>
    /// Event triggered when an item is added to the inventory.
    /// </summary>
    public event Action<IItem> OnItemAdded;

    /// <summary>
    /// Event triggered when an item is removed from the inventory.
    /// </summary>
    public event Action<IItem> OnItemRemoved;

    /// <summary>
    /// Gets a read-only collection of items in the inventory.
    /// </summary>
    public IReadOnlyList<IItem> Items => _items.AsReadOnly();

    /// <summary>
    /// The list of items in the inventory.
    /// </summary>
    private readonly List<IItem> _items = new();

    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if the item is null.</exception>
    public void AddItem(IItem item)
    {
        if (item == null)
        {
            GD.PrintErr("Cannot add a null item to the inventory.");
            return;
        }

        _items.Add(item);
        GD.Print($"Added {item.ItemName} to inventory.");
        OnItemAdded?.Invoke(item);
    }

    /// <summary>
    /// Adds multiple items to the inventory.
    /// </summary>
    /// <param name="items">The items to add.</param>
    public void AddItems(IEnumerable<IItem> items)
    {
        if (items == null)
        {
            GD.PrintErr("Cannot add a null collection of items to the inventory.");
            throw new ArgumentNullException(nameof(items));
        }

        foreach (var item in items)
        {
            AddItem(item);
        }
    }

    /// <summary>
    /// Removes an item from the inventory.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown if the item is null.</exception>
    public void RemoveItem(IItem item, MainCharacterTemplate character)
    {
        if (item == null)
        {
            GD.PrintErr("Cannot remove a null item from the inventory.");
            throw new ArgumentNullException(nameof(item));
        }

        if (_items.Contains(item))
        {
            _items.Remove(item);

            if (item is ArtifactModel artifact)
            {
                artifact.OnRemove(character);
            }

            OnItemRemoved?.Invoke(item);
        }
        else
        {
            GD.PrintErr($"Item {item.ItemName} not found in inventory.");
        }
    }

    /// <summary>
    /// Removes multiple items from the inventory.
    /// </summary>
    /// <param name="items">The items to remove.</param>
    public void RemoveItems(IEnumerable<IItem> items, MainCharacterTemplate character)
    {
        if (items == null)
        {
            GD.PrintErr("Cannot remove a null collection of items from the inventory.");
            throw new ArgumentNullException(nameof(items));
        }

        foreach (var item in items)
        {
            RemoveItem(item, character);
        }
    }

    /// <summary>
    /// Checks if the inventory contains a specific item.
    /// </summary>
    /// <param name="item">The item to check for.</param>
    /// <returns>True if the item is in the inventory, otherwise false.</returns>
    public bool HasItem(IItem item)
    {
        return item != null && _items.Contains(item);
    }

    /// <summary>
    /// Prints all items in the inventory to the console for debugging purposes.
    /// </summary>
    public void DebugPrint()
    {
        if (_items.Count == 0)
        {
            GD.Print("Inventory is empty.");
            return;
        }

        GD.Print("Inventory Items:");
        foreach (var item in _items)
        {
            GD.Print(item.ToString());
            if (item is ArtifactModel artifact)
            {
                GD.Print("Abilities:");
                foreach (var ability in artifact.Abilities)
                {
                    GD.Print($"- {ability.Description}");
                }
            }
        }
    }
}