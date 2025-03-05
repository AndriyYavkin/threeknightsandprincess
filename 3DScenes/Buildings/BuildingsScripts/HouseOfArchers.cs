using Game.HelperCharacters.Units;

namespace Game.Buildings;

public partial class HouseOfArchers : RecruitBuildingModel
{
    public override string BuildingName => "Archers house";
    public override string BuildingDescription => "Provide archers";
    public override Unit UnitToAdd => new Archer();
}