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
    public static Tile[,] Map { get; set; } 

    private float GridPositionConverter { get; set; } = 2f;

	public override void _Ready()
	{	
        AdjustGridMapCellSize();
		InitializeChildren();
		Pathfinder3D.Initialize(Map);
	}

	private void InitializeChildren()
	{
		InitializeStaticHelpers();

		foreach (Node child in GetChildren())
        {
            if (child is ScenesHelper.TileMapScripts.IMapInitializable initializable)
            {
                // Initialize the child with the Tile's values
                initializable.Initialize(MapWidth, MapHeight, GridPositionConverter);
                GD.Print($"Initialized {child.Name} with MapWidth={MapWidth}, MapHeight={MapHeight}, GridPositionConverter={GridPositionConverter}");
            }
        }
	}

	private void InitializeStaticHelpers()
	{
		TileMapHelper.MapWidth = MapWidth;
		TileMapHelper.MapHeight = MapHeight;
		TileMapHelper.GridPositionConverter = GridPositionConverter;

		CharacterHelper.MapWidth = MapWidth;
		CharacterHelper.MapHeight = MapHeight;
		CharacterHelper.GridPositionConverter = GridPositionConverter;
	}

    private void AdjustGridMapCellSize()
    {
        if (GridMapT == null)
        {
            GD.PrintErr("GridMap is not assigned!");
            return;
        }

        if(!GridMapT.CellSize.X.Equals(GridMapT.CellSize.Z))
        {
            GD.PrintErr($"GridMap x and z axis should be equal. Actual X: {GridMapT.CellSize.X}, Z: {GridMapT.CellSize.Z}. Default set: {GridPositionConverter}");
            return;
        }

        GridPositionConverter = GridMapT.CellSize.X;
    }
}
