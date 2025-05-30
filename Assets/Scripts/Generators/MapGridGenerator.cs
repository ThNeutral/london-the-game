using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGridGenerator : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    private GridController gridController;

    void Start()
    {
        gridController = GridController.Instance;

        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        var bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cellPos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile(cellPos);
                if (tile == null) continue;
                gridController.AddTerrainTile(cellPos, true);
            }
        }
    }
}