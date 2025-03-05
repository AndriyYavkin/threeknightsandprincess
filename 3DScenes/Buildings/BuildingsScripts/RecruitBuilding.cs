using Game.HelperCharacters;
using Game.HelperCharacters.Units;
using Godot;

namespace Game.Buildings;

/// <summary>
/// Represents a building that can be interacted with to add units to the player's army.
/// </summary>
public class RecruitBuildingModel
{
    /// <summary>
    /// Gets the buildings name.
    /// </summary>
    public string BuildingName { get; }

    /// <summary>
    /// Gets the buildings description.
    /// </summary>
    public string BuildingDescription { get; }

    /// <summary>
    /// The type of unit this building provides.
    /// </summary>
    public Unit UnitToAdd { get; }

    public int Amount { get; }

    public RecruitBuildingModel (string buildingName, string buildingDescription, int amount, Unit unitToAdd)
    {
        BuildingName = buildingName;
        BuildingDescription = buildingDescription;
        Amount = amount;
        UnitToAdd = unitToAdd;
    }
}