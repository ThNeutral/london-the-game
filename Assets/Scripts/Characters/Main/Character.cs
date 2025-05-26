using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IAbilityInteractable
{
    [SerializeField]
    private GameCharacterStats gameCharacterStats = new();
    public GameCharacterStats Stats { get => gameCharacterStats; }

    [SerializeField]
    private VisualCharacterStats visualCharacterStats = new();

    [SerializeField]
    private List<Ability> abilities = new();
    public Ability[] Abilities { get => abilities.ToArray(); }

    private List<Vector3> path;
    public List<Vector3> Path { set {  path = value; } }

    [SerializeField]
    private Side allegiance;
    public Side Allegiance { get => allegiance; }

    private Action onEndOfMovement;
    public Action OnEndOfMovement { set => onEndOfMovement += value; }

    void Start()
    {
        abilities.Insert(0, new Ability() { MoveDistance = -1, Type = AbilityType.MOVE, AbilityDistance = 0 });    
    }

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

    public void InteractWithAbility(AbilityMessage message)
    {
        if (message.From != allegiance && message.Ability.Type == AbilityType.DAMAGE)
        {
            ReceiveDamage(message.Ability.HitPointChange);
            Debug.Log(gameCharacterStats.HealthPoints);
        }
    }

    public void ReceiveDamage(int damage)
    {
        gameCharacterStats.HealthPoints -= damage;
        if (gameCharacterStats.HealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
