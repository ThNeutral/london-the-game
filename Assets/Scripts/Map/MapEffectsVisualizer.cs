using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEffectVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tile walkableTile;

    [SerializeField]
    private Tile nonWalkableTile;

    [SerializeField]
    private Tile attackTile;

    [SerializeField]
    private Tile moveTile;

    [SerializeField]
    private Tile selectTile;

    [SerializeField]
    private GridController gridController;

    private readonly HashSet<Vector3Int> affectedTiles = new();

    private void Start()
    {
        gridController = GridController.Instance;
    }

    public void AttackTiles(Vector3Int[] positions)
    {
        ChangeTiles(positions, attackTile);
    }
    public void MoveTiles(Vector3Int[] positions)
    {
        ChangeTiles(positions, moveTile);
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

    public Vector3 CellToWorld(Vector3Int world)
    {
        return tilemap.CellToWorld(world);
    }

    public Vector3 CellToWorldCentered(Vector3Int cell)
    {
        var worldPos = tilemap.CellToWorld(cell);
        var cellSize = tilemap.cellSize;
        cellSize.z = cellSize.y;
        cellSize.y = 0;
        var tileCenterOffset = cellSize * 0.5f;
        return worldPos + tileCenterOffset;
    }

    public void ResetTiles()
    {
        var openedTiles = new List<Vector3Int>();
        var closedTiles = new List<Vector3Int>();
        foreach (var pos in affectedTiles) 
        {
            Debug.Assert(gridController.TryGetTerrain(pos, out var tile));
            if (tile) openedTiles.Add(pos);
            else closedTiles.Add(pos);
        }

        ChangeTiles(openedTiles.ToArray(), walkableTile, true);
        ChangeTiles(closedTiles.ToArray(), nonWalkableTile, true);
    }
}