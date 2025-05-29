using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
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

    private Dictionary<Vector3Int, bool> characters;
    public Dictionary<Vector3Int, bool> Characters
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

    public Vector3Int[] FindPath(Vector3Int start, Vector3Int end)
    {
        return Pathfinder.FindPath(terrain, start, end);
    }

    public void MoveCharacter(Vector3Int from, Vector3Int to)
    {
        if (!terrain.ContainsKey(from) || !terrain.ContainsKey(to)) return;

        if (!characters.TryGetValue(from, out var fromCharacter) || !fromCharacter) return;

        characters.Remove(from);
        characters[to] = fromCharacter;
    }
}