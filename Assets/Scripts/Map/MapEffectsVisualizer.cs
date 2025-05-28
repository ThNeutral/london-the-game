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
    private Tile highlightTile;

    [SerializeField]
    private Tile selectTile;

    private readonly HashSet<Vector3Int> affectedTiles = new();

    public void HighlightTiles(Vector3Int[] positions)
    {
        ChangeTiles(positions, highlightTile);
    }

    public void SelectTiles(Vector3Int[] positions)
    {
        ChangeTiles(positions, selectTile);
    }

    private void ChangeTiles(Vector3Int[] positions, Tile tile, bool clearAffected = false)
    {
        if (clearAffected) affectedTiles.Clear();
        else affectedTiles.UnionWith(positions);
        var tiles = positions.Select((_) => tile).ToArray();
        tilemap.SetTiles(positions, tiles);    
    }

    public Vector3Int WorldToCell(Vector3 world)
    {
        return tilemap.WorldToCell(world);
    }

    public void ResetTiles()
    {
        ChangeTiles(affectedTiles.ToArray(), idleTile, true);
    }
}