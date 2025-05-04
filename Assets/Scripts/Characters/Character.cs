using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private List<Vector3> path;
    public List<Vector3> Path { set {  path = value; } }

    [SerializeField]
    private float movementSpeed = 1f;
    [SerializeField]
    private float movementEpsilon = 0.05f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (path == null || path.Count == 0) return;

        var target = path[0];
        var currentPosition = transform.position;
        target.y = currentPosition.y;

        transform.position = Vector3.MoveTowards(currentPosition, target, movementSpeed * Time.deltaTime);

        if (Vector3.Distance(currentPosition, target) < movementEpsilon)
        {
            path.RemoveAt(0);
        }
    }
}
