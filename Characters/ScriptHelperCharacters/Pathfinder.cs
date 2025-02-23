using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ScenesHelper;

namespace GameHelperCharacters;

public static class Pathfinder3D
{
    private static AStar3D astar = new AStar3D();
    private static HashSet<Vector3I> tilesWithObjects = new HashSet<Vector3I>();
    private static Tile[,] map;

    /// <summary>
    /// Initialize A* algorithm in order to maintain pathing for the character. Connects each point with adjacent points. Should be used only once per Map.
    /// </summary>
    /// <param name="map">The tile map.</param>
    public static void Initialize(Tile[,] map)
    {
        Pathfinder3D.map = map;
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int z = 0; z < map.GetLength(1); z++)
            {
                Vector3I position = new Vector3I(x, 0, z);
                if (map[x, z].IsPassable && map[x, z].Object == null)
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
                else if (map[x, z].Object != null)
                {
                    tilesWithObjects.Add(position);
                }
            }
        }
    }

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
    /// Finds the path to the position using A* algorithm.
    /// </summary>
    /// <param name="start"> Starting position. From what position we are finding path</param>
    /// <param name="end"> Ending path. To what position we are finding path</param>
    /// <returns></returns>
    public static List<Vector3I> FindPath(Vector3I start, Vector3I end)
    {
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
    
    private static void AstarHandleInit(Tile[,] map, int x, int z)
    {
        int id = GetId(x, z);
        astar.AddPoint(id, new Vector3(x, 0, z)); // Y-axis is always 0

        // Set the movement cost for the tile
        astar.SetPointWeightScale(id, map[x, z].MovementCost);

        ConnectIfValid(x, z, x - 1, z, map);
        ConnectIfValid(x, z, x, z - 1, map);
        ConnectIfValid(x, z, x + 1, z, map);
        ConnectIfValid(x, z, x, z + 1, map);

        // Connect diagonals if both orthogonal neighbors are passable
        ConnectIfValid(x, z, x - 1, z - 1, map, true);
        ConnectIfValid(x, z, x + 1, z - 1, map, true);
        ConnectIfValid(x, z, x - 1, z + 1, map, true);
        ConnectIfValid(x, z, x + 1, z + 1, map, true);
    }

    private static void ReconnectTile(Vector3I position)
    {
        int x = position.X;
        int z = position.Z;
        
        ConnectIfValid(x, z, x - 1, z, map);
        ConnectIfValid(x, z, x, z - 1, map);
        ConnectIfValid(x, z, x + 1, z, map);
        ConnectIfValid(x, z, x, z + 1, map);

        ConnectIfValid(x, z, x - 1, z - 1, map, true);
        ConnectIfValid(x, z, x + 1, z - 1, map, true);
        ConnectIfValid(x, z, x - 1, z + 1, map, true);
        ConnectIfValid(x, z, x + 1, z + 1, map, true);
    }

    private static void ConnectIfValid(int x1, int z1, int x2, int z2, Tile[,] map, bool diagonal = false)
    {
        if (x2 < 0 || z2 < 0 || x2 >= map.GetLength(0) || z2 >= map.GetLength(1))
            return; // Out of bounds
        
        if (tilesWithObjects.Contains(new Vector3I(x2, 0, z2)))
            return; // Don't connect to tiles with objects
        
        if (!map[x2, z2].IsPassable)
            return; // Don't connect to impassable tiles
        
        if (diagonal)
        {
            if (!map[x1, z2].IsPassable || !map[x2, z1].IsPassable)
                return; // Ensure both adjacent orthogonal tiles are passable for diagonal movement
        }
        
        int id1 = GetId(x1, z1);
        int id2 = GetId(x2, z2);
        
        if (astar.HasPoint(id2))
            astar.ConnectPoints(id1, id2);
    }

    private static int GetId(int x, int z)
    {
        return z * 10000 + x; // Unique ID for each tile (ignore Y-axis)
    }
}