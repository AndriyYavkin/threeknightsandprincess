using Godot;
using ScenesHelper;
using GameHelperCharacters;
using ScenesHelper.TileMapScripts;
using System;

namespace Scenes;

public partial class TileMap : Node3D
{
	[Export] public int MapWidth { get; set; }
    [Export] public int MapHeight { get; set; }
    [Export] public GridMap GridMapT { get; set; }

    /// <summary>
    /// The 2D array representing the map tiles.
    /// </summary>
    public static Tile[,] Map { get; set; } 

    /// <summary>
    /// The size of each grid cell. Default is 2f. Set by private method that reads GridMap
    /// </summary>
    private float GridPositionConverter { get; set; } = 2f;

	public override void _Ready()
	{	
        if (!ValidateMapDimensions())
        {
            GD.PrintErr("Invalid map dimensions. MapWidth and MapHeight must be positive.");
            return;
        }

        AdjustGridMapCellSize();
        InitializeChildren();
        Pathfinder3D.Initialize(Map);
	}

    /// <summary>
    /// Initializes all children that implement IMapInitializable.
    /// </summary>
	private void InitializeChildren()
	{
		InitializeStaticHelpers();

		foreach (Node child in GetChildren())
        {
            if (child is IMapInitializable initializable)
            {
                // Initialize the child with the Tile's values
                initializable.Initialize(MapWidth, MapHeight, GridPositionConverter);
                GD.Print($"Initialized {child.Name} with MapWidth={MapWidth}, MapHeight={MapHeight}, GridPositionConverter={GridPositionConverter}");
            }
        }
	}

    /// <summary>
    /// Initializes static helper classes with map dimensions and grid size.
    /// </summary>
	private void InitializeStaticHelpers()
	{
		TileMapHelper.MapWidth = MapWidth;
		TileMapHelper.MapHeight = MapHeight;
		TileMapHelper.GridPositionConverter = GridPositionConverter;

		CharacterHelper.MapWidth = MapWidth;
		CharacterHelper.MapHeight = MapHeight;
		CharacterHelper.GridPositionConverter = GridPositionConverter;
	}

    /// <summary>
    /// Adjusts the grid position converter based on the GridMap's cell size.
    /// </summary>
    private void AdjustGridMapCellSize()
    {
        if (GridMapT == null)
        {
            GD.PrintErr("GridMap is not assigned!");
            return;
        }

        if (!GridMapT.CellSize.X.Equals(GridMapT.CellSize.Z))
        {
            GD.PrintErr($"GridMap x and z axis should be equal. Actual X: {GridMapT.CellSize.X}, Z: {GridMapT.CellSize.Z}. Default set: {GridPositionConverter}");
            return;
        }

        GridPositionConverter = GridMapT.CellSize.X;
        GD.Print($"Adjusted GridPositionConverter to {GridPositionConverter} based on GridMap cell size.");
    }

    /// <summary>
    /// Validates that MapWidth and MapHeight are positive.
    /// </summary>
    /// <returns>True if the dimensions are valid, otherwise false.</returns>
    private bool ValidateMapDimensions()
    {
        return MapWidth > 0 && MapHeight > 0;
    }
}
