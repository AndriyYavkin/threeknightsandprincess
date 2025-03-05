using Game.HelperCharacters;
using Godot;

namespace Game.Buildings;

public partial class BaseBuildingsScript : BuildingsRegister , IBuilding
{
	[Export] public Buildings Building { get; set; }
	public RecruitBuildingModel recruitBuildingModel{ get; set; }

	public string GetTitleUI() => "Building Info";
    public string GetNameUI() => recruitBuildingModel.BuildingName;
    // public Texture2D GetIconUI() => new Texture2D();/*GD.Load<Texture2D>("res://Textures/Character.png")*/
    public string GetDescriptionUI() => recruitBuildingModel.BuildingDescription;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BuildingsLookup.TryGetValue(Building, out var buildingsLookup);
		recruitBuildingModel = buildingsLookup;
	}

	public void OnInteract(CharacterHeroTemplate character)
	{
		if (character.Army.Units.Count >= character.Army.MaxCapacity)
        {
            GD.PrintErr("Army is at full capacity!");
            return;
        }

		character.Army.AddUnit(recruitBuildingModel.UnitToAdd, recruitBuildingModel.Amount);
		GD.Print($"Added {recruitBuildingModel.Amount} {recruitBuildingModel.UnitToAdd.Name} to {character.CharacterName}'s army.");
	}
}
