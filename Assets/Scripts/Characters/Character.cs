using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private GameCharacterStats gameCharacterStats = new();
    public GameCharacterStats Stats { get => gameCharacterStats; }

    [SerializeField]
    private VisualCharacterStats visualCharacterStats = new();

    private List<Vector3> path;
    public List<Vector3> Path { set {  path = value; } }

    [SerializeField]
    private Side allegiance;
    public Side Allegiance { get => allegiance; }

    private Action onEndOfMovement;
    public Action OnEndOfMovement { set => onEndOfMovement += value; }

    // Update is called once per frame
    void Update()
    {
        if (path == null) return;

        var target = path[0];
        var currentPosition = transform.position;
        target.y = currentPosition.y;

        var maxDelta = visualCharacterStats.MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(currentPosition, target, maxDelta);

        if (Vector3.Distance(currentPosition, target) < visualCharacterStats.MovementEpsilon)
        {
            path.RemoveAt(0);
        }

        if (path.Count == 0)
        {
            onEndOfMovement?.Invoke();
            onEndOfMovement = null;
            path = null;
        }
    }
}
