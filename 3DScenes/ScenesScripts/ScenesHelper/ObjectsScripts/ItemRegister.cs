using System.Collections.Generic;
using System.Linq;
using Godot;
using Objects;

namespace ScenesHelper.ObjectsHelper;

/// <summary>
/// Register of all the items
/// </summary>
public abstract partial class ItemRegistry : MeshInstance3D
{
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

    /// <summary>
    /// A dictionary to map ObjectType to the corresponding item lookup dictionary.
    /// </summary>
    protected static readonly Dictionary<ObjectType, Dictionary<object, IItem>> TypeLookup = new()
    {
        { ObjectType.Artifact, ArtifactLookup.ToDictionary(kvp => (object)kvp.Key, kvp => kvp.Value) },
        { ObjectType.Resource, ResourceLookup.ToDictionary(kvp => (object)kvp.Key, kvp => kvp.Value) },
    };
}