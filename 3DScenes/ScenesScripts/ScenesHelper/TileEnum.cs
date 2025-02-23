namespace ScenesHelper;

/// <summary>
/// All the tile types we have. If we want to add any more, we should firstly add here for example "Lava" and then in Tile class maintain it's characteristics
/// </summary>
public enum TileType
{
    NotDefined,
    Grass,
    Water,
    Mountain,
    Forest,
    Town,
    Road,
}