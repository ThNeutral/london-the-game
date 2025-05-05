using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisualizer : MonoBehaviour
{
    public static string WALL_TILE = "wall";
    public static string FLOOR_TILE = "floor";
    public static string HIGHLIGHT_ALL_PATHS_TILE = "highlight_all_paths";
    public static string HIGHLIGHT_SELECTED_PATH_TILE = "highlight_selected_path";

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

    public void HighlightTiles(Vector3Int[] tiles, string type, bool clear = true)
    {
        if (clear) effects.ClearAllTiles();

        if (this.tiles.TryGetValue(type, out var value)) {
            var highlight = this.tiles[type];
            var highlights = tiles.Select(_ => highlight).ToArray();
            effects.SetTiles(tiles, highlights);
        }
        else
        {
            Debug.LogError($"Invalid tile type: '{type}'");
        }
    }

    public void ClearEffects()
    {
        effects.ClearAllTiles();
    }
}
