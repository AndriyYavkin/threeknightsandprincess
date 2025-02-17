using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ScenesHelper;

namespace GameHelperCharacters;

public static class Pathfinder3D
{
    private static AStar3D astar = new AStar3D();

    /// <summary>
    /// initialize A* algorithm in order to maintain pathing for the character. Connects each point with adjacent points. Should be used only once per Node
    /// </summary>
    /// <param name="map"></param>
    public static void Initialize(Tile[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int z = 0; z < map.GetLength(1); z++)
            {
                if (map[x, z].IsPassable) // Ignore Y-axis
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
            }
        }
    }

    /// <summary>
    /// Finds the path to the position using A* algorithm.
    /// </summary>
    /// <param name="start"> Starting position. From what position we are finding path</param>
    /// <param name="end"> Ending path. To what position we are finding path</param>
    /// <returns></returns>
    public static List<Vector3I> FindPath(Vector3I start, Vector3I end)
    {
        int startId = GetId(start.X, start.Z);
        int endId = GetId(end.X, end.Z);

        var path = astar.GetPointPath(startId, endId);
        return path.Select(p => new Vector3I((int)p.X, 0, (int)p.Z)).ToList();
    }
    
    private static void AstarHandleInit(Tile[,] map, int x, int z)
    {
        int id = GetId(x, z);
        astar.AddPoint(id, new Vector3(x, 0, z)); // Y-axis is always 0

        if (x > 0 && map[x - 1, z].IsPassable)
        {
            int adjacentId = GetId(x - 1, z);
            if (astar.HasPoint(adjacentId))
                astar.ConnectPoints(id, adjacentId);
        }
        if (z > 0 && map[x, z - 1].IsPassable)
        {
            int adjacentId = GetId(x, z - 1);
            if (astar.HasPoint(adjacentId))
                astar.ConnectPoints(id, adjacentId);
        }
        if (x < map.GetLength(0) - 1 && map[x + 1, z].IsPassable)
        {
            int adjacentId = GetId(x + 1, z);
            if (astar.HasPoint(adjacentId))
                astar.ConnectPoints(id, adjacentId);
        }
        if (z < map.GetLength(1) - 1 && map[x, z + 1].IsPassable)
        {
            int adjacentId = GetId(x, z + 1);
            if (astar.HasPoint(adjacentId))
                astar.ConnectPoints(id, adjacentId);
        }

        // Connect to diagonal tiles only if both orthogonal tiles are passable
        if (x > 0 && z > 0 && map[x - 1, z].IsPassable && map[x, z - 1].IsPassable && map[x - 1, z - 1].IsPassable)
        {
            int diagonalId = GetId(x - 1, z - 1);
            if (astar.HasPoint(diagonalId))
                astar.ConnectPoints(id, diagonalId);
        }
        if (x < map.GetLength(0) - 1 && z > 0 && map[x + 1, z].IsPassable && map[x, z - 1].IsPassable && map[x + 1, z - 1].IsPassable)
        {
            int diagonalId = GetId(x + 1, z - 1);
            if (astar.HasPoint(diagonalId))
                astar.ConnectPoints(id, diagonalId);
        }
        if (x > 0 && z < map.GetLength(1) - 1 && map[x - 1, z].IsPassable && map[x, z + 1].IsPassable && map[x - 1, z + 1].IsPassable)
        {
            int diagonalId = GetId(x - 1, z + 1);
            if (astar.HasPoint(diagonalId))
                astar.ConnectPoints(id, diagonalId);
        }
        if (x < map.GetLength(0) - 1 && z < map.GetLength(1) - 1 && map[x + 1, z].IsPassable && map[x, z + 1].IsPassable && map[x + 1, z + 1].IsPassable)
        {
            int diagonalId = GetId(x + 1, z + 1);
            if (astar.HasPoint(diagonalId))
                astar.ConnectPoints(id, diagonalId);
        }
    }

    private static int GetId(int x, int z)
    {
        return z * 10000 + x; // Unique ID for each tile (ignore Y-axis)
    }
}