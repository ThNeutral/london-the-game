using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.TextCore.Text;
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

    private Vector3Int startMovement = new(int.MaxValue, int.MaxValue, int.MaxValue);
    private Vector3Int endMovement = new(int.MaxValue, int.MaxValue, int.MaxValue);
    private HashSet<Vector3Int> allPossiblePathTiles;
    private List<Vector3Int> selectedPath;

    private Vector3Int startAttack = new(int.MaxValue, int.MaxValue, int.MaxValue);
    private Vector3Int endAttack = new(int.MaxValue, int.MaxValue, int.MaxValue);
    private HashSet<Vector3Int> allPossibleAttackTiles;
    private List<Vector3Int> selectedAttack;

    private Vector3Int EmptyTile { get => new(int.MaxValue, int.MaxValue, int.MaxValue); }

    private void Start()
    {
    }

    public Vector3Int WorldToGridIndex(Vector3 screen)
    {
        return visualizer.WorldToGridIndex(screen);
    }

    public void UpdateMovementEndTile(Vector3Int tile)
    {
        if (tile != endMovement && 
            allPossiblePathTiles != null && 
            allPossiblePathTiles.Contains(tile) &&
            (tile == startMovement || !characters.ContainsKey(tile))
        ) {
            endMovement = tile;
            selectedPath = FindPath(startMovement, endMovement);

            visualizer.ClearEffects();
            visualizer.HighlightTiles(allPossiblePathTiles.ToArray(), VisualizerTileType.AllPath);
            visualizer.HighlightTiles(selectedPath.ToArray(), VisualizerTileType.SelectedPath);
            visualizer.HighlightTiles(allPossibleAttackTiles.ToArray(), VisualizerTileType.Attack);
        }
    }

    public void UpdateAttackEndTile(Vector3Int tile)
    {
        if (tile != endAttack &&
            allPossibleAttackTiles != null &&
            allPossibleAttackTiles.Contains(tile)
        )
        {
            endAttack = tile;
            selectedAttack = FindPath(startAttack, endAttack, true);

            visualizer.ClearEffects();
            visualizer.HighlightTiles(allPossibleAttackTiles.ToArray(), VisualizerTileType.Attack);
            visualizer.HighlightTiles(selectedAttack.ToArray(), VisualizerTileType.SelectedPath);
        }
    }

    public void SelectInitialTile(Vector3Int tile) 
    {
        if (!characters.TryGetValue(tile, out var character)) return;
        if (!turnController.IsAllowedToMove(character)) return;

        if (grid.TryGetValue(tile, out var type))
        {
            if (type == TileType.Wall) return;
            visualizer.HighlightTiles(new Vector3Int[] { tile }, VisualizerTileType.SelectedPath);
            startMovement = tile;
        }
    }

    public void ShowAbilityTiles(Ability ability)
    {
        var character = characters[startMovement];
        if (!character.Abilities.Contains(ability))
        {
            Debug.LogError("Selected ability is not present in characters ability list");
        }

        var movementDistance = character.Stats.MovementDistance;
        if (ability.Type != AbilityType.MOVE && ability.MoveDistance >= 0)
        {
            movementDistance = ability.MoveDistance;
        }

        var abilityDistance = ability.AbilityDistance;
        if (abilityDistance < 0)
        {
            abilityDistance = 0;
        }

        allPossiblePathTiles = FindAllPossiblePaths(startMovement, movementDistance);
        allPossibleAttackTiles = new HashSet<Vector3Int>(FindAllPossiblePaths(startMovement, movementDistance + abilityDistance).Except(allPossiblePathTiles));

        visualizer.ClearEffects();
        visualizer.HighlightTiles(allPossiblePathTiles.ToArray(), VisualizerTileType.AllPath);
        visualizer.HighlightTiles(new Vector3Int[] { startMovement }, VisualizerTileType.SelectedPath);
        visualizer.HighlightTiles(allPossibleAttackTiles.ToArray(), VisualizerTileType.Attack);
    }

    public bool GetCharacter(Vector3Int tile, out Character character)
    {
        return characters.TryGetValue(tile, out character);
    }

    public bool RollbackMovement()
    {
        var exists = characters.TryGetValue(endMovement, out var character);
        if (!exists) return false;

        var copy = new List<Vector3Int>(selectedPath);
        copy.Reverse();

        characters.Remove(endMovement);
        characters[startMovement] = character;

        character.Path = visualizer.GridIndexesToWorldCentered(copy);

        visualizer.ClearEffects();

        return true;
    }

    public bool CommitMovement(Ability ability)
    {
        if (selectedPath != null)
        {
            var character = characters[startMovement];
            if (!character.Abilities.Contains(ability))
            {
                Debug.LogError("Selected ability is not present in characters ability list");
            }

            characters.Remove(startMovement);
            characters[endMovement] = character;

            character.Path = visualizer.GridIndexesToWorldCentered(selectedPath);

            if (ability.Type == AbilityType.MOVE)
            {
                visualizer.ClearEffects();
                turnController.Move(character);
                return true;
            }

            startAttack = endMovement;

            var abilityDistance = ability.AbilityDistance;
            if (abilityDistance < 0)
            {
                abilityDistance = 0;
            }

            allPossibleAttackTiles = FindAllPossiblePaths(startAttack, abilityDistance, true);

            visualizer.ClearEffects();
            visualizer.HighlightTiles(allPossibleAttackTiles.ToArray(), VisualizerTileType.Attack);

            return true;
        }

        return false;
    }

    public void CommitAction(Ability ability)
    {
        var character = characters[endMovement];
        if (!character.Abilities.Contains(ability))
        {
            Debug.LogError("Selected ability is not present in characters ability list");
        }

        if (characters.TryGetValue(endAttack, out var targetCharacter))
        {
            targetCharacter.InteractWithAbility(AbilityMessage.Generate(ability, character));
        }

        startMovement = EmptyTile;
        endMovement = EmptyTile;

        allPossiblePathTiles = null;
        allPossibleAttackTiles = null;

        turnController.Move(character);
        visualizer.ClearEffects();
    }

    private HashSet<Vector3Int> FindAllPossiblePaths(Vector3Int start, int length, bool ignoreEnemy = false)
    {
        var path = new HashSet<Vector3Int>();
        if (grid.TryGetValue(start, out var type) && type == TileType.Wall) return path;

        characters.TryGetValue(start, out var character);
        DFS(start, path, length + 1, character, ignoreEnemy);

        return path;
    }

    private void DFS(Vector3Int current, HashSet<Vector3Int> path, int stepsLeft, Character character, bool ignoreEnemy)
    {
        if (stepsLeft == 0) return;

        var hasEnemy = characters.TryGetValue(current, out var neighbor) && neighbor.Allegiance != character.Allegiance;
        if ((hasEnemy && !ignoreEnemy) || !grid.TryGetValue(current, out var tileType) || tileType == TileType.Wall) return;

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
            DFS(current + dir, path, stepsLeft - 1, character, ignoreEnemy);
        }
    }

    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, bool ignoreEnemy = false)
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
                if (walkable && (!containsEnemy || ignoreEnemy))
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
