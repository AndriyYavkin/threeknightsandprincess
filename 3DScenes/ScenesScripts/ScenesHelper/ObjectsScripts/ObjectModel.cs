using Godot;
using GameHelperCharacters;

namespace ScenesHelper.ObjectsHelper;

public abstract partial class ObjectModel : ItemRegistry, IObjectPickable
{
    /// <summary>
    /// The type of the object.
    /// </summary>
    public abstract ObjectType Type { get; set; }

    /// <summary>
    /// The specific artifact type.
    /// </summary>
    public abstract ArtifactType Artifact { get; set; }

    /// <summary>
    /// The specific resource type.
    /// </summary>
    public abstract ResourceType Resource { get; set; }

    /// <summary>
    /// The linked item that will be added to the inventory when picked up.
    /// </summary>
    public IItem LinkedItem { get; set; }

    public override void _Ready()
    {
        GetLinkedItem();
    }

    public void OnPickUp(CharacterBody3D character)
    {
        if (character is ICharacterTemplate characterTemplate && LinkedItem != null)
        {
            // Add the linked item to the character's inventory
            characterTemplate.Inventory.AddItem(LinkedItem);
            GD.Print($"{LinkedItem.ItemName} picked up by {character.Name}.");
            QueueFree();
        }
    }

    public override string ToString()
    {
        switch(Type)
        {
            case ObjectType.Artifact: return $"Type of the item is {Type}. It's name {Artifact}";
            case ObjectType.Resource: return $"Type of the item is {Type}. It's name {Resource}";
        }

        return base.ToString();
    }

    protected void GetLinkedItem()
    {
        // Initialize the linked item based on the type
        if (TypeLookup.TryGetValue(Type, out var lookup) && lookup.TryGetValue(GetKey(), out var item))
        {
            LinkedItem = item;
            GD.Print($"{LinkedItem.ItemName} is ready for pickup.");
            return;
        }

        GD.PrintErr("Failed to initialize the linked item.");
    }

    private object GetKey()
    {
        object key = Type switch
        {
            ObjectType.Artifact => Artifact,
            ObjectType.Resource => Resource,
            _ => throw new System.ArgumentOutOfRangeException(nameof(Type), "Item was not defined!")
         };

         return key;
    }
}