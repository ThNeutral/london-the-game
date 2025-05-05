using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField]
    private Bounds actualSize;

    [SerializeField]
    private Vector2 tileSize;

    [SerializeField]
    private GameObject tile;
    private List<List<GameObject>> tiles;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var xCount = Mathf.FloorToInt(actualSize.size.x / tileSize.x);
        var zCount = Mathf.FloorToInt(actualSize.size.z / tileSize.y);

        var bottomLeft = actualSize.min;
        
        tiles = new(xCount); 
        for (int x = 0; x < xCount; x++)
        {
            var row = new List<GameObject>(zCount);
            for (int z = 0; z < zCount; z++)
            {
                var tilePos = new Vector3(
                    bottomLeft.x + x * tileSize.x + tileSize.x / 2f,
                    actualSize.center.y,
                    bottomLeft.z + z * tileSize.y + tileSize.y / 2f + 0.6f
                );

                GameObject newTile = Instantiate(tile, tilePos, Quaternion.identity, transform);
                row.Add(newTile);
            }
            tiles.Add(row);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
