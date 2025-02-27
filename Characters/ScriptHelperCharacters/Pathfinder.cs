using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ScenesHelper;

namespace GameHelperCharacters;

/// <summary>
/// Provides pathfinding functionality using the A* algorithm for a 3D grid-based map.
/// </summary>
public static class Pathfinder3D
{
    private static AStar3D astar = new AStar3D();
    private static HashSet<Vector3I> tilesWithObjects = new HashSet<Vector3I>();
    private static Tile[,] map;

    /// <summary>
    /// A constant used to generate unique IDs for grid positions.
    /// </summary>
    private const int IdMultiplier = 10000;

    /// <summary>
    /// Initializes the A* algorithm with the provided tile map. Connects each passable tile with its adjacent tiles.
    /// </summary>
    /// <param name="map">The tile map to initialize the pathfinder with.</param>
    public static void Initialize(Tile[,] map)
    {
        if (map == null)
        {
            GD.PrintErr("Map is null! Pathfinder initialization failed.");
            return;
        }

        Pathfinder3D.map = map;

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int z = 0; z < map.GetLength(1); z++)
            {
                Vector3I position = new Vector3I(x, 0, z);
                if (map[x, z].GetPassable() && map[x, z].ContainsObject == null)
                {
                    try
                    {
                        AstarHandleInit(map, x, z);
                    }
                    catch (Exception ex)
                    {
                        GD.PrintErr(ex);
                    }
                }
                else if (map[x, z].ContainsObject != null)
                {
                    tilesWithObjects.Add(position);
                }
            }
        }
    }

    /// <summary>
    /// Updates the passability state of a tile in the pathfinder.
    /// </summary>
    /// <param name="position">The position of the tile to update.</param>
    /// <param name="isPassable">Whether the tile is passable.</param>
    public static void UpdateTileState(Vector3I position, bool isPassable)
    {
        int tileId = GetId(position.X, position.Z);
        if (isPassable)
        {
            tilesWithObjects.Remove(position);
            if (!astar.HasPoint(tileId))
            {
                astar.AddPoint(tileId, new Vector3(position.X, 0, position.Z));
                ReconnectTile(position);
            }
        }
        else
        {
            tilesWithObjects.Add(position);
            astar.RemovePoint(tileId);
        }
    }

    /// <summary>
    /// Finds a path from the start position to the end position using the A* algorithm.
    /// </summary>
    /// <param name="start">The starting position of the path.</param>
    /// <param name="end">The ending position of the path.</param>
    /// <returns>A list of positions representing the path, or an empty list if no path is found.</returns>
    public static List<Vector3I> FindPath(Vector3I start, Vector3I end)
    {
        if (map == null)
        {
            GD.PrintErr("Map is not initialized!");
            return new List<Vector3I>();
        }

        if (!IsPositionValid(start) || !IsPositionValid(end))
        {
            GD.PrintErr("Start or end position is out of bounds!");
            return new List<Vector3I>();
        }

        int startId = GetId(start.X, start.Z);
        int endId = GetId(end.X, end.Z);

        if (!astar.HasPoint(startId) || (!astar.HasPoint(endId) && !tilesWithObjects.Contains(end)))
            return new List<Vector3I>();

        if (tilesWithObjects.Contains(end))
        {
            astar.AddPoint(endId, new Vector3(end.X, 0, end.Z));
            ReconnectTile(end);
        }
        
        var path = astar.GetPointPath(startId, endId);
        
        if (tilesWithObjects.Contains(end))
        {
            astar.RemovePoint(endId);
        }

        return path.Select(p => new Vector3I((int)p.X, 0, (int)p.Z)).ToList();
    }
    
    /// <summary>
    /// Initializes a tile in the A* grid and connects it to its neighbors.
    /// </summary>
    /// <param name="map">The tile map.</param>
    /// <param name="x">The x-coordinate of the tile.</param>
    /// <param name="z">The z-coordinate of the tile.</param>
    private static void AstarHandleInit(Tile[,] map, int x, int z)
    {
        int id = GetId(x, z);
        astar.AddPoint(id, new Vector3(x, 0, z)); // Y-axis is always 0

        // Set the movement cost for the tile
        astar.SetPointWeightScale(id, map[x, z].GetMovementCost());

        ConnectOrthogonalNeighbors(x, z, map);
        ConnectDiagonalNeighbors(x, z, map);
    }


    /// <summary>
    /// Reconnects a tile to its neighbors after its state changes.
    /// </summary>
    /// <param name="position">The position of the tile to reconnect.</param>
    private static void ReconnectTile(Vector3I position)
    {
        int x = position.X;
        int z = position.Z;

        ConnectOrthogonalNeighbors(x, z, map);
        ConnectDiagonalNeighbors(x, z, map);
    }

    /// <summary>
    /// Connects a tile to its orthogonal neighbors if they are valid.
    /// </summary>
    /// <param name="x1">The x-coordinate of the tile.</param>
    /// <param name="z1">The z-coordinate of the tile.</param>
    /// <param name="map">The tile map.</param>
    private static void ConnectOrthogonalNeighbors(int x1, int z1, Tile[,] map)
    {
        ConnectIfValid(x1, z1, x1 - 1, z1, map);
        ConnectIfValid(x1, z1, x1, z1 - 1, map);
        ConnectIfValid(x1, z1, x1 + 1, z1, map);
        ConnectIfValid(x1, z1, x1, z1 + 1, map);
    }

    /// <summary>
    /// Connects a tile to its diagonal neighbors if they are valid.
    /// </summary>
    /// <param name="x1">The x-coordinate of the tile.</param>
    /// <param name="z1">The z-coordinate of the tile.</param>
    /// <param name="map">The tile map.</param>
    private static void ConnectDiagonalNeighbors(int x1, int z1, Tile[,] map)
    {
        ConnectIfValid(x1, z1, x1 - 1, z1 - 1, map, true);
        ConnectIfValid(x1, z1, x1 + 1, z1 - 1, map, true);
        ConnectIfValid(x1, z1, x1 - 1, z1 + 1, map, true);
        ConnectIfValid(x1, z1, x1 + 1, z1 + 1, map, true);
    }

    /// <summary>
    /// Connects two tiles if they are valid and passable.
    /// </summary>
    /// <param name="x1">The x-coordinate of the first tile.</param>
    /// <param name="z1">The z-coordinate of the first tile.</param>
    /// <param name="x2">The x-coordinate of the second tile.</param>
    /// <param name="z2">The z-coordinate of the second tile.</param>
    /// <param name="map">The tile map.</param>
    /// <param name="diagonal">Whether the connection is diagonal.</param>
    private static void ConnectIfValid(int x1, int z1, int x2, int z2, Tile[,] map, bool diagonal = false)
    {
        if (x2 < 0 || z2 < 0 || x2 >= map.GetLength(0) || z2 >= map.GetLength(1))
            return; // Out of bounds

        if (tilesWithObjects.Contains(new Vector3I(x2, 0, z2)))
            return; // Don't connect to tiles with objects

        if (!map[x2, z2].GetPassable())
            return; // Don't connect to impassable tiles

        if (diagonal && !map[x1, z2].GetPassable() || !map[x2, z1].GetPassable())
            return;

        int id1 = GetId(x1, z1);
        int id2 = GetId(x2, z2);

        if (astar.HasPoint(id2))
            astar.ConnectPoints(id1, id2);
    }

    /// <summary>
    /// Generates a unique ID for a tile based on its coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate of the tile.</param>
    /// <param name="z">The z-coordinate of the tile.</param>
    /// <returns>A unique ID for the tile.</returns>
    private static int GetId(int x, int z)
    {
        return z * IdMultiplier + x; // Unique ID for each tile (ignore Y-axis)
    }

    /// <summary>
    /// Checks if a position is within the bounds of the map.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True if the position is valid, otherwise false.</returns>
    private static bool IsPositionValid(Vector3I position)
    {
        return position.X >= 0 && position.X < map.GetLength(0) &&
               position.Z >= 0 && position.Z < map.GetLength(1);
    }
}