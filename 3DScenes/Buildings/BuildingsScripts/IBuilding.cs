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
    public Buildings Building { get; set; }
	public RecruitBuildingModel recruitBuildingModel{ get; set; }
}