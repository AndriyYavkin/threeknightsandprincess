using Game.HelperCharacters.Units;

namespace Game.Buildings;

public partial class KnightsHouse : RecruitBuildingModel
{
    public override string BuildingName => "Knights house";
    public override string BuildingDescription => "Provide knights";
    public override Unit UnitToAdd => new Knight();
}