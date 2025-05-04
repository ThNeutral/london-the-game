using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class GridController : MonoBehaviour
{
    [SerializeField]
    private GridVisualizer visualizer;

    private Dictionary<Vector3Int, TileType> grid = new();
    public Dictionary<Vector3Int, TileType> Grid { set { visualizer.BuildTilemap(value); grid = value; } }

    private Dictionary<Vector3Int, Character> characters = new();

    [SerializeField]
    private Vector2Int TEST_ONLYCharacterInitialPosition;

    [SerializeField]
    private GameObject TEST_ONLYCharacterPrefab;

    private Vector3Int start;
    private HashSet<Vector3Int> path;

    private void Start()
    {
        var pos = visualizer.GridIndexToWorldCentered((Vector3Int)TEST_ONLYCharacterInitialPosition);
        pos.y = 0.5f;
        characters.Add(
            (Vector3Int)TEST_ONLYCharacterInitialPosition,
            Instantiate(TEST_ONLYCharacterPrefab, pos, Quaternion.identity).GetComponent<Character>()
        );
    }

    public Vector3Int WorldToGridIndex(Vector3 screen)
    {
        return visualizer.WorldToGridIndex(screen);
    }

    public void HandleTileClick(Vector3Int tile)
    {
        if (path == null)
        {
            if (grid.TryGetValue(tile, out var type) && characters.TryGetValue(tile, out var _))
            {
                if (type == TileType.Wall) return;
                path = FindAllPossiblePaths(tile, 5);
                visualizer.HighlightTiles(path.ToArray());
                start = tile;
            }
        }
        else
        {
            if (path.Contains(tile))
            {
                var character = characters[start];
                characters.Remove(start);
                characters[tile] = character;

                var path = FindPath(start, tile);
                if (path != null)
                {
                    Debug.Log(path.Count);
                    character.Path = visualizer.GridIndexesToWorldCentered(path);
                }
            }

            visualizer.ClearEffects();
            path = null;
        }
    }

    private HashSet<Vector3Int> FindAllPossiblePaths(Vector3Int start, int length)
    {
        var path = new HashSet<Vector3Int>();
        if (grid.TryGetValue(start, out var type) && type == TileType.Wall) return path;

        DFS(start, path, length);

        return path;
    }

    private void DFS(Vector3Int current, HashSet<Vector3Int> path, int stepsLeft)
    {
        if (stepsLeft < 0 || path.Contains(current)) return;
        if (!grid.TryGetValue(current, out var tileType) || tileType == TileType.Wall) return;

        path.Add(current);

        var directions = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

        foreach (var dir in directions)
        {
            DFS(current + dir, path, stepsLeft - 1);
        }
    }

    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var visited = new HashSet<Vector3Int>();
        var queue = new Queue<Vector3Int>();

        if (!grid.TryGetValue(start, out var startType) || startType == TileType.Wall) return null;
        if (!grid.TryGetValue(goal, out var goalType) || goalType == TileType.Wall) return null;

        queue.Enqueue(start);
        visited.Add(start);

        var directions = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current == goal)
            {
                // Reconstruct path
                var path = new List<Vector3Int> { goal };
                while (current != start)
                {
                    current = cameFrom[current];
                    path.Add(current);
                }
                path.Reverse();
                return path;
            }

            foreach (var dir in directions)
            {
                var neighbor = current + dir;
                if (visited.Contains(neighbor)) continue;

                if (grid.TryGetValue(neighbor, out var type) && type != TileType.Wall)
                {
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return null;
    }
}
