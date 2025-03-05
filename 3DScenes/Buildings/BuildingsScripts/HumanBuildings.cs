using Game.HelperCharacters.Units;

namespace Game.Buildings;

public class Farm : RecruitBuildingModel
{
    public Farm() : base("Farm", "Provide food and free meat-shields", 15, new Peasant())
    {
    }
}

public class HouseOfArchers : RecruitBuildingModel
{
    public HouseOfArchers() : base("Archers house", "Provide archers", 10, new Archer())
    {
    }
}

public class KnightsHouse : RecruitBuildingModel
{
    public KnightsHouse() : base("Knights house", "Provide knights", 5, new Knight())
    {
    }
}