using System.Collections.Generic;
using System.Linq;
using Godot;
using ScenesHelper;

namespace GameHelperCharacters;

public static class Pathfinder3D
{
    private static AStar3D astar = new AStar3D();

    public static void Initialize(Tile[,,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int z = 0; z < map.GetLength(2); z++)
            {
                if (map[x, 0, z].IsPassable) // Ignore Y-axis
                {
                    int id = GetId(x, z);
                    astar.AddPoint(id, new Vector3(x, 0, z)); // Y-axis is always 0

                    // Connect to adjacent passable tiles (ignore Y-axis)
                    if (x > 0 && map[x - 1, 0, z].IsPassable)
                        astar.ConnectPoints(id, GetId(x - 1, z));
                    if (z > 0 && map[x, 0, z - 1].IsPassable)
                        astar.ConnectPoints(id, GetId(x, z - 1));
                }
            }
        }
    }

    public static List<Vector3I> FindPath(Vector3I start, Vector3I end)
    {
        int startId = GetId(start.X, start.Z);
        int endId = GetId(end.X, end.Z);

        var path = astar.GetPointPath(startId, endId);
        return path.Select(p => new Vector3I((int)p.X, (int)p.Y, (int)p.Z)).ToList();
    }

    private static int GetId(int x, int z)
    {
        return z * 1000 + x; // Unique ID for each tile (ignore Y-axis)
    }
}