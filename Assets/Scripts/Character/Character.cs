using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static readonly float SpeedCoefficient = 1f;

    [SerializeField]
    private CharacterStats stats;
    public CharacterStats Stats { get => stats; }

    [SerializeField]
    private List<CharacterAbility> abilities;
    public List<CharacterAbility> Abilities { get => abilities; }

    private readonly List<Vector3> path = new();

    private MessageBus bus;

    private void Start()
    {
        bus = MessageBus.Instance;
    }

    public List<Vector3> Path
    {
        set
        {
            foreach (var tile in value)
            {
                path.Add(tile);
            }
        }
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (path.Count == 0) return;

        var nextNode = path[0];
        if (Vector3.Distance(transform.position, nextNode) < 0.01f)
        {
            path.RemoveAt(0);
            if (path.Count == 0)
            {
                return;
            }
            nextNode = path[0];
        }

        transform.position = Vector3.MoveTowards(transform.position, nextNode, stats.Speed * SpeedCoefficient * Time.deltaTime);
    }

    public void ReceiveDamage(int damage)
    {
        stats.Health -= damage;
        if (stats.Health <= 0)
        {
            bus.Publish(MessageBus.EventType.CharacterDeath, this);
            Destroy(gameObject);
        }
    }
}