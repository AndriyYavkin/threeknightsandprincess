namespace ScenesHelper.TileMapScripts;

public interface IMapInitializable
{
    int MapWidth { get; set; }
    int MapHeight { get; set; }
    float GridPositionConverter { get; set; }

    void Initialize(int mapWidth, int mapHeight, float gridPositionConverter);
}