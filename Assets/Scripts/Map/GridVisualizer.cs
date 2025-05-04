using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisualizer : MonoBehaviour
{
    public static string WALL_TILE = "wall";
    public static string FLOOR_TILE = "floor";
    public static string HIGHLIGHT_TILE = "highlight";

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tilemap effects;

    private Dictionary<string, Tile> tiles;
    public Dictionary<string, Tile> Tiles { set { tiles = value; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildTilemap(Dictionary<Vector3Int, TileType> tiles)
    {
        tilemap.ClearAllTiles();
        foreach (var kvp in tiles)
        {
            Tile tile = null;
            if (kvp.Value == TileType.Floor)
            {
                tile = this.tiles[FLOOR_TILE];
            }
            else if (kvp.Value == TileType.Wall)
            {
                tile = this.tiles[WALL_TILE];
            }
            else
            {
                Debug.LogError($"Invalid tile type {kvp.Value}");
            }

            tilemap.SetTile(kvp.Key, tile);
        }
    }

    public Vector3Int WorldToGridIndex(Vector3 world)
    {
        return tilemap.WorldToCell(world);
    }

    public Vector3 GridIndexToWorld(Vector3Int grid)
    {
        return tilemap.CellToWorld(grid);
    }

    public List<Vector3> GridIndexesToWorldCentered(List<Vector3Int> indexes)
    {
        return indexes.Select(index => GridIndexToWorldCentered(index)).ToList();
    }
    public Vector3 GridIndexToWorldCentered(Vector3Int grid)
    {
        var cellPosition = tilemap.CellToWorld(grid);
        var cellSize = tilemap.layoutGrid.cellSize;
        cellSize.z = cellSize.y; cellSize.y = 0;
        return cellPosition + (cellSize / 2f);
    }

    public void HighlightTile(Vector3Int tile, bool clear = true)
    {
        if (clear) effects.ClearAllTiles();

        var highlight = tiles[HIGHLIGHT_TILE];
        effects.SetTile(tile, highlight);
    }

    public void HighlightTiles(Vector3Int[] tiles, bool clear = true)
    {
        if (clear) effects.ClearAllTiles();

        var highlight = this.tiles[HIGHLIGHT_TILE];
        var highlights = tiles.Select(_ => highlight).ToArray();
        effects.SetTiles(tiles, highlights);
    }

    public void ClearEffects()
    {
        effects.ClearAllTiles();
    }
}
