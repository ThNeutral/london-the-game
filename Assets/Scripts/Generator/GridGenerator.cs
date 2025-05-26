using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


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
    private Tile highlightAttack;

    [SerializeField]
    private GameObject allyPrefab;

    [SerializeField]
    private Tile ally;

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private Tile enemy;

    void Start()
    {
        var grid = new Dictionary<Vector3Int, TileType>();
        var characters = new Dictionary<Vector3Int, GameObject>();

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
                    else if (tile.name == ally.name)
                    {
                        characters[pos] = allyPrefab;
                        grid[pos] = TileType.Floor;
                    }
                    else if (tile.name == enemy.name)
                    {
                        characters[pos] = enemyPrefab;
                        grid[pos] = TileType.Floor;
                    }
                    else
                    {
                        Debug.LogError($"Unknown texture name {tile.name}");
                    }
                }
            }
        }

        var transparency = 0.3f;
        visualizer.Tiles = new Dictionary<VisualizerTileType, Tile> 
        {
            { VisualizerTileType.Floor, GetTransparentTile(floor, transparency) },
            { VisualizerTileType.Wall, GetTransparentTile(wall, transparency) },
            { VisualizerTileType.AllPath, GetTransparentTile(highlightAllPaths, transparency) },
            { VisualizerTileType.SelectedPath, GetTransparentTile(highlightSelectedPath, transparency) },
            { VisualizerTileType.Attack, GetTransparentTile(highlightAttack, transparency) }
        };

        gridController.Grid = grid;

        foreach (var character in characters)
        {
            var pos = visualizer.GridIndexToWorldCentered(character.Key); pos.y = 0.5f;
            var behaviour = Instantiate(character.Value, pos, Quaternion.identity).GetComponent<Character>();
            gridController.Charaters.Add(character.Key, behaviour);
            
            turnController.Turns[behaviour.Allegiance][behaviour] = true;
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
