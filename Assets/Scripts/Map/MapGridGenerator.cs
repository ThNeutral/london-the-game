using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGridGenerator : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private GridController gridController;

    [SerializeField]
    private MapEffectVisualizer visualizer;

    [SerializeField]
    private GameObject allyPrefab;

    void Start()
    {
        GenerateTerrain();
        GenerateCharacterPositions();
    }

    private void GenerateTerrain()
    {
        var terrain = new Dictionary<Vector3Int, bool>();
        var bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cellPos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile(cellPos);
                if (tile == null) continue;
                terrain.Add(cellPos, true);
            }
        }
        gridController.Terrain = terrain;
    }

    private void GenerateCharacterPositions()
    {
        var tile = new Vector3Int(-2, -2);
        var character = Instantiate(allyPrefab).GetComponent<Character>();

        var world = visualizer.CellToWorldCentered(tile);
        world.y += 1f;
        character.transform.position = world;

        var characters = new Dictionary<Vector3Int, Character>
        {
            { tile, character }
        };
        gridController.Characters = characters;
    }
}