using Game.HelperCharacters.Units;

namespace Game.Buildings;

public partial class Farm : RecruitBuildingModel
{
    public override string BuildingName => "Farm";
    public override string BuildingDescription => "Provide food and free meat-shields";
    public override Unit UnitToAdd => new Peasant();
}