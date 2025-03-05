using System.Collections.Generic;
using System.Linq;
using Godot;
using Game.Objects;

namespace Game.ScenesHelper.ObjectsHelper;

/// <summary>
/// Represents a registry of all objects in the game, including artifacts and resources.
/// </summary>
public abstract partial class ItemRegistry : Node3D
{
    /// <summary>
    /// A dictionary to map artifact types to their corresponding item instances.
    /// </summary>
    protected static readonly Dictionary<ArtifactType, IItem> ArtifactLookup = new()
    {
        { ArtifactType.BootsOfSpeed, new BootsOfSpeed() },
        { ArtifactType.SecondArtifact, new SecondArtifact() }, 
    };

    /// <summary>
    /// A dictionary to map resource types to their corresponding item instances.
    /// </summary>
    protected static readonly Dictionary<ResourceType, IItem> ResourceLookup = new()
    {
        { ResourceType.Gold, new ResourceGold() },
    };

    /// <summary>
    /// A dictionary to map ObjectType to the corresponding item lookup dictionary.
    /// </summary>
    protected static readonly Dictionary<ObjectType, Dictionary<object, IItem>> TypeLookup = new()
    {
        { ObjectType.Artifact, ArtifactLookup.ToDictionary(kvp => (object)kvp.Key, kvp => kvp.Value) },
        { ObjectType.Resource, ResourceLookup.ToDictionary(kvp => (object)kvp.Key, kvp => kvp.Value) },
    };
}