using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGridGenerator : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private GridController gridController;

    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        var grid = new Dictionary<Vector3Int, bool>();
        var bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cellPos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile(cellPos);
                if (tile == null) continue;
                grid.Add(cellPos, true);
            }
        }
        gridController.Grid = grid;
    }
}