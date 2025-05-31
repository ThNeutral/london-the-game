using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridController : MonoBehaviour
{
    private static GridController _instance;
    public static GridController Instance
    {
        get
        {
            if (_instance == null) Debug.LogError("Grid Controller was not created in the scene.");
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("Multiple Grid Controller instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
    }

    [SerializeField]
    private MapEffectVisualizer visualizer;

    private readonly Dictionary<Vector3Int, bool> terrain = new();
    public void AddTerrainTile(Vector3Int pos, bool tile)
    {
        terrain.Add(pos, tile);
    }

    private readonly Dictionary<Vector3Int, Character> characters = new();
    public void AddCharacter(Vector3Int pos, Character character)
    {
        Debug.Assert(terrain.ContainsValue(character), "Given character already exists in grid");
        characters.Add(pos, character);
    }

    public bool TryGetCharacter(Vector3Int pos, out Character character)
    {
        return characters.TryGetValue(pos, out character);
    }

    private TurnController turnController;

    private void Start()
    {
        turnController = TurnController.Instance;
    }

    public Vector3Int[] FindPath(Vector3Int start, Vector3Int end, Character character = null)
    {
        if (character == null) return Pathfinder.FindPath(terrain, start, end);

        var grid = GetWalkableTiles(character);
        return Pathfinder.FindPath(grid, start, end);
    }

    private Dictionary<Vector3Int, bool> GetWalkableTiles(Character character)
    {
        var grid = new Dictionary<Vector3Int, bool>(terrain);
        var charData = turnController.GetCharacterTurnData(character);
        foreach (var kvp in terrain)
        {
            if (characters.TryGetValue(kvp.Key, out var characterOnTile))
            {
                var charOnTileData = turnController.GetCharacterTurnData(characterOnTile);
                grid[kvp.Key] = grid[kvp.Key] && (charData.side == charOnTileData.side);
            }
        }
        return grid;
    }

    public bool TryMoveCharacter(Vector3Int from, Vector3Int to)
    {
        if (!terrain.ContainsKey(from) || !terrain.ContainsKey(to)) return false;

        return TryMoveCharacter(FindPath(from, to));
    }

    public bool TryMoveCharacter(Vector3Int[] path)
    {
        Debug.Assert(path != null && path.Length > 2, "Invalid path");

        if (!characters.TryGetValue(path.First(), out var character) || !character) return false;

        var worldPath = new List<Vector3>(path.Length);
        foreach (var pos in path)
        {
            var world = visualizer.CellToWorldCentered(pos);
            world.y += 1f;
            worldPath.Add(world);
        }

        characters.Remove(path.First());
        characters[path.Last()] = character;
        character.Path = worldPath;

        return true;
    }
}