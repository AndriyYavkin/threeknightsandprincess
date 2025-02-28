using System.Collections.Generic;
using Objects;
using Godot;
using GameHelperCharacters;

namespace ScenesHelper.ObjectsHelper;

public abstract partial class ObjectModel : MeshInstance3D, IObjectPickable
{
    /// <summary>
    /// The type of the object.
    /// </summary>
    public abstract ObjectType Type { get; set; }

    /// <summary>
    /// The specific artifact type (visible only if Type is Artifact).
    /// </summary>
    public abstract ArtifactType Artifact { get; set; }

    /// <summary>
    /// The specific resource type (visible only if Type is Resource).
    /// </summary>
    public abstract ResourceType Resource { get; set; }

    /// <summary>
    /// The linked item that will be added to the inventory when picked up.
    /// </summary>
    public IItem LinkedItem { get; set; }

    /// <summary>
    /// A dictionary to map artifact types to their corresponding item instances.
    /// </summary>
    protected static readonly Dictionary<ArtifactType, IItem> ArtifactLookup = new()
    {
        { ArtifactType.FirstArtifact, new FirstArtifact() },
    };

    /// <summary>
    /// A dictionary to map resource types to their corresponding item instances.
    /// </summary>
    protected static readonly Dictionary<ResourceType, IItem> ResourceLookup = new()
    {
        { ResourceType.Gold, new ResourceGold() },
    };

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
        switch (Type)
        {
            case ObjectType.Artifact:
                if (ArtifactLookup.TryGetValue(Artifact, out var artifact))
                {
                    LinkedItem = artifact;
                }
                break;

            case ObjectType.Resource:
                if (ResourceLookup.TryGetValue(Resource, out var resource))
                {
                    LinkedItem = resource;
                }
                break;
        }

        if (LinkedItem != null)
        {
            GD.Print($"{LinkedItem.ItemName} is ready for pickup.");
        }
        else
        {
            GD.PrintErr("Failed to initialize the linked item.");
        }
    }
}