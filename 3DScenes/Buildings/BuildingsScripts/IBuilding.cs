using Game.HelperCharacters;
using Game.ScenesHelper;
using Game.UI;
using Godot;

namespace Game.Buildings;

/// <summary>
/// Defines an interface for objects that can be interacted with.
/// </summary>
public interface IBuilding : IUIDisplayable , IInteractable
{
    /// <summary>
    /// Gets or sets building name.
    /// </summary>
    string BuildingName { get; }

    /// <summary>
    /// Gets or sets building description.
    /// </summary>
    string BuildingDescription { get; }
}