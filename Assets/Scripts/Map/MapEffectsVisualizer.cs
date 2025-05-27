using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEffectVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tile idleTile;

    [SerializeField]
    private Tile hoveredTile;

    public void HighlightTiles(Vector3Int[] positions)
    {
        var tiles = positions.Select((_) => hoveredTile).ToArray();
        tilemap.SetTiles(positions, tiles);
    }

    public Vector3Int WorldToCell(Vector3 world)
    {
        return tilemap.WorldToCell(world);
    }
}