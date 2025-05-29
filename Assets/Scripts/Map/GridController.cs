using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField]
    private MapEffectVisualizer visualizer;

    private Dictionary<Vector3Int, bool> terrain;
    public Dictionary<Vector3Int, bool> Terrain
    {
        set
        {
            if (terrain != null)
            {
                Debug.LogError($"{nameof(terrain)} is already set");
                return;
            }
            terrain = value;
        }

        get => terrain;
    }

    private Dictionary<Vector3Int, Character> characters;
    public Dictionary<Vector3Int, Character> Characters
    {
        set
        {
            if (characters != null)
            {
                Debug.LogError($"{nameof(characters)} is already set");
                return;
            }

            var invalidPositions = new List<Vector3Int>();
            foreach (var kvp in value)
            {
                if (!terrain.ContainsKey(kvp.Key))
                {
                    invalidPositions.Add(kvp.Key);
                }
            }

            if (invalidPositions.Count != 0)
            {
                Debug.LogError($"Positions {invalidPositions.Select((pos, index) => $"{pos}{(index != invalidPositions.Count ? "," : "")}")} do not exist in terrain");
                return;
            }

            characters = value;
        }

        get => characters;
    }

    public bool TryGetCharacter(Vector3Int pos, out Character character)
    {
        return characters.TryGetValue(pos, out character);
    }

    public Vector3Int[] FindPath(Vector3Int start, Vector3Int end)
    {
        return Pathfinder.FindPath(terrain, start, end);
    }

    public bool TryMoveCharacter(Vector3Int from, Vector3Int to, Vector3Int[] path = null)
    {
        if (!terrain.ContainsKey(from) || !terrain.ContainsKey(to)) return false;

        if (!characters.TryGetValue(from, out var character) || !character) return false;

        path ??= FindPath(from, to);
        if (path == null) return false;

        var worldPath = new List<Vector3>(path.Length);
        foreach (var pos in path)
        {
            var world = visualizer.CellToWorldCentered(pos);
            world.y += 1f;
            worldPath.Add(world);   
        }

        characters.Remove(from);
        characters[to] = character;
        character.Path = worldPath;

        return true;
    }
}