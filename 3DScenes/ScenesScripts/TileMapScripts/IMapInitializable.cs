namespace ScenesHelper.TileMapScripts;

/// <summary>
/// You should use this interface if you need to initializate a new tile map. Don't forget to put TilesMap on the top of other children in godot itself!
/// </summary>
public interface IMapInitializable
{
    /// <summary>
    /// Map width. Better be set by class that have [Export] MapWidth
    /// </summary>
    int MapWidth { get; set; }
    /// <summary>
    /// Map height. Better be set by class that have [Export] MapHeight;
    /// </summary>
    int MapHeight { get; set; }
    /// <summary>
    /// Size of the grids. Better be between 0.5f and 2f
    /// </summary>
    float GridPositionConverter { get; set; }

    /// <summary>
    /// Initialize classes with need in map size and grid size
    /// </summary>
    void Initialize(int mapWidth, int mapHeight, float gridPositionConverter);
}