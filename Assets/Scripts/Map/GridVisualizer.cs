using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum VisualizerTileType
{
    Wall,
    Floor,
    AllPath,
    SelectedPath,
    Attack,
}

public class GridVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tilemap effects;

    private Dictionary<VisualizerTileType, Tile> tiles;
    public Dictionary<VisualizerTileType, Tile> Tiles { set { tiles = value; } }

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
                tile = this.tiles[VisualizerTileType.Floor];
            }
            else if (kvp.Value == TileType.Wall)
            {
                tile = this.tiles[VisualizerTileType.Wall];
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

    public void ClearHighlightsTilemap()
    {
        effects.ClearAllTiles();
    }

    public void HighlightTiles(Vector3Int[] tiles, VisualizerTileType type)
    {
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
