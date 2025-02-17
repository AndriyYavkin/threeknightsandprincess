namespace ScenesHelper.TileMapScripts;

/// <summary>
/// You should use this interface if you need to initializate a new tile map. Don't forget to put TilesMap on the top of other children in godot itself!
/// </summary>
public interface IMapInitializable
{
    int MapWidth { get; set; }
    int MapHeight { get; set; }
    float GridPositionConverter { get; set; }

    void Initialize(int mapWidth, int mapHeight, float gridPositionConverter);
}