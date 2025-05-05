using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TEST_ONLYCharacterData
{
    public GameObject prefab;
    public Vector2Int position;
}

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private GridController gridController;

    [SerializeField]
    private GridVisualizer visualizer;

    [SerializeField]
    private TurnController turnController;

    [SerializeField]
    private Tilemap source;

    [SerializeField]
    private Tile wall;

    [SerializeField]
    private Tile floor;

    [SerializeField]
    private Tile highlightAllPaths;

    [SerializeField]
    private Tile highlightSelectedPath;

    [SerializeField]
    private TEST_ONLYCharacterData[] datas;

    void Start()
    {
        var grid = new Dictionary<Vector3Int, TileType>();

        var bounds = source.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                var tileBase = source.GetTile(pos);
                if (tileBase != null && tileBase is Tile tile)
                {
                    if (tile.name == floor.name) 
                    {
                        grid[pos] = TileType.Floor;
                    } 
                    else if (tile.name == wall.name)
                    {
                        grid[pos] = TileType.Wall;
                    }
                    else
                    {
                        Debug.LogError($"Unknown texture name {tile.name}");
                    }
                }
            }
        }

        visualizer.Tiles = new Dictionary<string, Tile> 
        {
            { GridVisualizer.FLOOR_TILE, GetTransparentTile(floor, 0.3f) },
            { GridVisualizer.WALL_TILE, GetTransparentTile(wall, 0.3f) },
            { GridVisualizer.HIGHLIGHT_ALL_PATHS_TILE, GetTransparentTile(highlightAllPaths, 0.3f) },
            { GridVisualizer.HIGHLIGHT_SELECTED_PATH_TILE, GetTransparentTile(highlightSelectedPath, 0.3f) },
        };

        gridController.Grid = grid;

        foreach (var data in datas)
        {
            var pos = visualizer.GridIndexToWorldCentered((Vector3Int)data.position); pos.y = 0.5f;
            var character = Instantiate(data.prefab, pos, Quaternion.identity).GetComponent<Character>();
            gridController.Charaters.Add((Vector3Int)data.position, character);
            
            turnController.Turns[character.Allegiance][character] = true;

        }

        source.gameObject.SetActive(false);
    }

    private Tile GetTransparentTile(Tile source, float A)
    {
        Tile newTile = ScriptableObject.CreateInstance<Tile>();
        newTile.sprite = source.sprite;
        var old = source.color;
        newTile.color = new Color(old.r, old.g, old.b, A);
        newTile.colliderType = source.colliderType;
        newTile.flags = source.flags;
        newTile.gameObject = source.gameObject;
        newTile.transform = source.transform;

        return newTile;
    }
}
