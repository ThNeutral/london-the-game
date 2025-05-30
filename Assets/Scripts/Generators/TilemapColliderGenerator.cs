using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapColliderGenerator : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private new MeshCollider collider;

    void Start()
    {
        RegenerateCollider();
    }

    // TODO: optimize mesh generation: combine quads into bigger ones were possible
    public void RegenerateCollider()
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        var bounds = tilemap.cellBounds;
        var index = 0;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cellPos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile(cellPos);

                if (tile == null) continue;
                var worldPos = tilemap.CellToWorld(cellPos);
                worldPos.y = worldPos.z;
                worldPos.z = gameObject.transform.position.z;

                // Quad vertices (clockwise)
                vertices.Add(worldPos);
                vertices.Add(worldPos + new Vector3(1, 0));
                vertices.Add(worldPos + new Vector3(1, 1));
                vertices.Add(worldPos + new Vector3(0, 1));

                // Two triangles per quad
                triangles.Add(index);
                triangles.Add(index + 2);
                triangles.Add(index + 1);

                triangles.Add(index);
                triangles.Add(index + 3);
                triangles.Add(index + 2);

                index += 4;
            }
        }

        var mesh = new Mesh()
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray()
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        collider.sharedMesh = null;
        collider.sharedMesh = mesh;
    }
}
