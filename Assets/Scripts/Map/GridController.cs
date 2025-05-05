using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class GridController : MonoBehaviour
{
    [SerializeField]
    private GridVisualizer visualizer;

    [SerializeField]
    private GridUI gridUI;

    [SerializeField]
    private TurnController turnController;

    private Dictionary<Vector3Int, TileType> grid = new();
    public Dictionary<Vector3Int, TileType> Grid { set { visualizer.BuildTilemap(value); grid = value; } }

    private Dictionary<Vector3Int, Character> characters = new();
    public Dictionary<Vector3Int, Character> Charaters { get => characters; }

    private Vector3Int start = new(int.MaxValue, int.MaxValue, int.MaxValue);
    private Vector3Int end = new(int.MaxValue, int.MaxValue, int.MaxValue);
    private HashSet<Vector3Int> allPossiblePathTiles;
    private List<Vector3Int> selectedPath;

    private void Start()
    {
    }

    public Vector3Int WorldToGridIndex(Vector3 screen)
    {
        return visualizer.WorldToGridIndex(screen);
    }

    public void HandleTileHover(Vector3Int tile)
    {
        if (tile != end && 
            allPossiblePathTiles != null && 
            allPossiblePathTiles.Contains(tile) &&
            !characters.ContainsKey(tile)
        ) {
            end = tile;
            selectedPath = FindPath(start, end);
            visualizer.HighlightTiles(allPossiblePathTiles.ToArray(), GridVisualizer.HIGHLIGHT_ALL_PATHS_TILE);
            visualizer.HighlightTiles(selectedPath.ToArray(), GridVisualizer.HIGHLIGHT_SELECTED_PATH_TILE, false);
        }
    }

    public void HandleTileClick(Vector3Int tile)
    {
        if (allPossiblePathTiles == null) HandleNullPath(tile);
        else HandleSelectedPath();
    }

    private void HandleNullPath(Vector3Int tile) 
    {
        if (!characters.TryGetValue(tile, out var character)) return;
        if (!turnController.IsAllowedToMove(character)) return;

        if (grid.TryGetValue(tile, out var type))
        {
            if (type == TileType.Wall) return;
            allPossiblePathTiles = FindAllPossiblePaths(tile, character.Stats.MovementDistance);
            visualizer.HighlightTiles(allPossiblePathTiles.ToArray(), GridVisualizer.HIGHLIGHT_ALL_PATHS_TILE);
            start = tile;
        }
    }

    private void HandleSelectedPath() 
    {
        if (selectedPath != null && start != end)
        {
            var character = characters[start];
            characters.Remove(start);
            characters[end] = character;
            character.Path = visualizer.GridIndexesToWorldCentered(selectedPath);
            selectedPath = null;

            turnController.Move(character);

            gridUI.EnableGridUI = false;
            character.OnEndOfMovement = () => gridUI.EnableGridUI = true;
        }

        start = new(int.MaxValue, int.MaxValue, int.MaxValue);
        end = new(int.MaxValue, int.MaxValue, int.MaxValue);
        visualizer.ClearEffects();
        allPossiblePathTiles = null;
    }

    private HashSet<Vector3Int> FindAllPossiblePaths(Vector3Int start, int length)
    {
        var path = new HashSet<Vector3Int>();
        if (grid.TryGetValue(start, out var type) && type == TileType.Wall) return path;

        characters.TryGetValue(start, out var character);
        DFS(start, path, length, character);

        return path;
    }

    private void DFS(Vector3Int current, HashSet<Vector3Int> path, int stepsLeft, Character character)
    {
        if (stepsLeft == 0) return;

        var hasEnemy = characters.TryGetValue(current, out var neighbor) && neighbor.Allegiance != character.Allegiance;
        if (hasEnemy || !grid.TryGetValue(current, out var tileType) || tileType == TileType.Wall) return;

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
            DFS(current + dir, path, stepsLeft - 1, character);
        }
    }

    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var visited = new HashSet<Vector3Int>();
        var queue = new Queue<Vector3Int>();
        var startHasCharacter = characters.TryGetValue(start, out var character);

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

                var walkable = grid.TryGetValue(neighbor, out var type) && type != TileType.Wall;
                var containsEnemy = characters.TryGetValue(neighbor, out var neighborCharacter) && neighborCharacter.Allegiance != character.Allegiance;
                if (walkable && !containsEnemy)
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
