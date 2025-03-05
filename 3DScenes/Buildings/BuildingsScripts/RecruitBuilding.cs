using Game.HelperCharacters;
using Game.HelperCharacters.Units;
using Godot;

namespace Game.Buildings;

/// <summary>
/// Represents a building that can be interacted with to add units to the player's army.
/// </summary>
public abstract partial class RecruitBuildingModel : Node3D, IBuilding
{
    public abstract string BuildingName { get; }
    public abstract string BuildingDescription { get; }

    /// <summary>
    /// The type of unit this building provides.
    /// </summary>
    public abstract Unit UnitToAdd { get; }

    /// <summary>
    /// Called when the building is interacted with.
    /// </summary>
    /// <param name="character">The character interacting with the building.</param>
    public void OnInteract(CharacterHeroTemplate character)
    {
        if (UnitToAdd != null)
        {
            character.Army.AddUnit(UnitToAdd);
            GD.Print($"{UnitToAdd.Name} added to {character.CharacterName}'s army.");
        }
        else
        {
            GD.PrintErr("No unit to add!");
        }
    }

    public string GetTitleUI() => "Resource Info";
    public string GetNameUI() => BuildingName;
    //public Texture2D GetIconUI() => new Texture2D();/*GD.Load<Texture2D>("res://Textures/Character.png")*/
    public string GetDescriptionUI() => BuildingDescription;
}