using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static readonly float SpeedCoefficient = 1f;

    [SerializeField]
    private CharacterStats stats;

    private List<Vector3> path;
    public List<Vector3> Path
    {
        set => path = value;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (path == null) return;

        var nextNode = path[0];
        if (Vector3.Distance(transform.position, nextNode) < 0.05f)
        {
            path.RemoveAt(0);
            if (path.Count == 0)
            {
                path = null;
                return;
            }
            nextNode = path[0];
        }

        transform.position = Vector3.MoveTowards(transform.position, nextNode, stats.Speed * SpeedCoefficient * Time.deltaTime);
    }
}