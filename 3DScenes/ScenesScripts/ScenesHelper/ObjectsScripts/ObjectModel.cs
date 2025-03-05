using Godot;
using Game.HelperCharacters;

namespace Game.ScenesHelper.ObjectsHelper;

/// <summary>
/// Represents an object that can be picked up by a character and added to their inventory.
/// </summary>
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

    public string GetTitleUI() => "Resource Info";
    public string GetNameUI() => LinkedItem.ItemName;
    //public Texture2D GetIconUI() => new Texture2D();/*GD.Load<Texture2D>("res://Textures/Character.png")*/
    public string GetDescriptionUI() => LinkedItem.Description;

    public override void _Ready()
    {
        GetLinkedItem();
    }

    /// <summary>
    /// Called when the object is picked up by a character.
    /// </summary>
    /// <param name="character">The character that picked up the object.</param>
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

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    /// <returns>A string describing the object's type and name.</returns>
    public override string ToString()
    {
        switch(Type)
        {
            case ObjectType.Artifact: return $"Type of the item is {Type}. It's name {Artifact}";
            case ObjectType.Resource: return $"Type of the item is {Type}. It's name {Resource}";
        }

        return base.ToString();
    }

    /// <summary>
    /// Initializes the linked item based on the object's type.
    /// </summary>
    protected void GetLinkedItem()
    {
        // Initialize the linked item based on the type
        if (TypeLookup.TryGetValue(Type, out var lookup) && lookup.TryGetValue(GetKey(), out var item))
        {
            LinkedItem = item;
            return;
        }
        GD.PrintErr($"Failed to initialize the linked item. {Type}");
    }

    /// <summary>
    /// Gets the key for the linked item based on the object's type.
    /// </summary>
    /// <returns>The key for the linked item.</returns>
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