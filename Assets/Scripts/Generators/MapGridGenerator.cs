using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGridGenerator : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tile walkableTile;

    [SerializeField]
    private Tile nonWalkableTile;

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

                if (tile.name == walkableTile.name)
                {
                    gridController.AddTerrainTile(cellPos, true);
                }
                else if (tile.name == nonWalkableTile.name)
                {
                    gridController.AddTerrainTile(cellPos, false);
                }
                else
                {
                    Debug.LogError($"Found invalid tile on grid.");
                }
            }
        }
    }
}