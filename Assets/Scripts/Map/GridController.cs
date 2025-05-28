using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    private Dictionary<Vector3Int, bool> grid;
    public Dictionary<Vector3Int, bool> Grid
    {
        set
        {
            if (grid != null)
            {
                Debug.LogError("Grid is already set");
                return;
            }
            grid = value;
        }
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        return Pathfinder.FindPath(grid, start, end);
    }
}