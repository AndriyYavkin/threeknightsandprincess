using Godot;

namespace Game.ScenesHelper.TileMapScripts;

/// <summary>
/// Defines an interface for initializing a new tile map. Ensures that the TilesMap node is placed above other children in the Godot scene tree for proper initialization.
/// </summary>
public interface IMapInitializable
{
    /// <summary>
    /// Gets or sets the width of the map. It is recommended to set this via an [Export] property in the implementing class.
    /// </summary>
    int MapWidth { get; set; }

    /// <summary>
    /// Gets or sets the height of the map. It is recommended to set this via an [Export] property in the implementing class.
    /// </summary>
    int MapHeight { get; set; }

    /// <summary>
    /// Gets or sets the size of each grid cell. Recommended values are greater than 1.0.
    /// </summary>
    float GridPositionConverter { get; set; }

    /// <summary>
    /// Initializes the map with the specified dimensions and grid size.
    /// </summary>
    /// <param name="mapWidth">The width of the map. Must be a positive integer.</param>
    /// <param name="mapHeight">The height of the map. Must be a positive integer.</param>
    /// <param name="gridPositionConverter">The size of each grid cell. Recommended values are greater than 1.0.</param>
    /// <exception cref="System.ArgumentException">Thrown if mapWidth, mapHeight, or gridSize are invalid.</exception>
    void Initialize(int mapWidth, int mapHeight, float gridPositionConverter);
}