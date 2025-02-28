using System.Runtime.CompilerServices;
using Godot;

namespace ScenesHelper.ObjectsHelper;

public interface IItem
{
    /// <summary>
    /// Name of the Item
    /// </summary>
    string ItemName { get; }

    /// <summary>
    /// Descripton of the Item
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Method to pick up the item
    /// </summary>
    /// <param name="character">Who picked up the item</param>
    void PickUp(CharacterBody3D character);
}