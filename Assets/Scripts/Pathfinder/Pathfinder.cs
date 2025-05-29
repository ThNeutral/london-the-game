using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    public static Vector3Int[] FindPath(Dictionary<Vector3Int, bool> grid, Vector3Int start, Vector3Int end)
    {
        var openSet = new PriorityQueue<Vector3Int>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        var gScore = new Dictionary<Vector3Int, int>();
        var fScore = new Dictionary<Vector3Int, int>();

        foreach (var cell in grid.Keys)
        {
            gScore[cell] = int.MaxValue;
            fScore[cell] = int.MaxValue;
        }

        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);
        openSet.Enqueue(start, fScore[start]);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == end)
                return ReconstructPath(cameFrom, current).ToArray();

            foreach (var neighbor in GetNeighbors(current, grid))
            {
                int tentativeGScore = gScore[current] + 1;

                if (tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Heuristic(neighbor, end);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return null; // No path found
    }

    private static int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z); // Manhattan in 3D
    }

    private static List<Vector3Int> GetNeighbors(Vector3Int current, Dictionary<Vector3Int, bool> grid)
    {
        var directions = new[]
        {
            new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)
        };

        var neighbors = new List<Vector3Int>();
        foreach (var dir in directions)
        {
            var neighbor = current + dir;
            if (grid.TryGetValue(neighbor, out bool walkable) && walkable)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        var path = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private class PriorityQueue<T>
    {
        private readonly List<KeyValuePair<T, int>> elements = new List<KeyValuePair<T, int>>();

        public int Count => elements.Count;

        public void Enqueue(T item, int priority)
        {
            elements.Add(new KeyValuePair<T, int>(item, priority));
            elements.Sort((a, b) => a.Value.CompareTo(b.Value));
        }

        public T Dequeue()
        {
            var item = elements[0].Key;
            elements.RemoveAt(0);
            return item;
        }

        public bool Contains(T item)
        {
            return elements.Exists(e => EqualityComparer<T>.Default.Equals(e.Key, item));
        }
    }
}