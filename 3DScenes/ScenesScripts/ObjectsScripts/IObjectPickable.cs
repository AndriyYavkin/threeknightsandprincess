using Godot;
using Game.UI;

namespace Game.ScenesHelper.ObjectsHelper;

/// <summary>
/// Represents an object that can be picked up by a character.
/// </summary>
public interface IObjectPickable : IUIDisplayable , IInteractable
{
    /// <summary>
    /// The linked item that will be added to the inventory when picked up.
    /// </summary>
    public IItem LinkedItem { get; set; }
}
