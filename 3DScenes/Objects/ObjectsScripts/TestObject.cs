using Godot;
using ScenesHelper.ObjectsHelper;

namespace ObjectsScripts;

public partial class TestObject : ObjectModel
{
    /// <summary>
    /// The type of the object.
    /// </summary>
    [Export] public override ObjectType Type {get; set;}

    /// <summary>
    /// The specific artifact type.
    /// </summary>
    [Export] public override ArtifactType Artifact { get; set; }

    /// <summary>
    /// The specific resource type.
    /// </summary>
    [Export] public override ResourceType Resource { get; set; }
}
