using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurnController : MonoBehaviour
{
    [SerializeField]
    private bool isPlayerFirstToMove = true;

    private Dictionary<Side, Dictionary<Character, bool>> turns = new() {
        {Side.Player, new()},
        {Side.AI, new()}
    };
    public Dictionary<Side, Dictionary<Character, bool>> Turns { get => turns; }

    private Side currentTurn;
    public Side CurrentTurn { get => currentTurn; }
    
    void Start()
    {
        if (isPlayerFirstToMove)
        {
            currentTurn = Side.Player;
        }
        else 
        {
            currentTurn = Side.AI;
        }
    }

    public void Move(Character character)
    {
        if (!IsAllowedToMove(character)) return;
        turns[currentTurn][character] = false;

        if (HasAllMoved())
        {
            ResetTurns();
            NextTurn();
        }
    }

    private void NextTurn()
    {
        currentTurn.Next();
    }

    public bool IsAllowedToMove(Character character)
    {
        return turns[currentTurn].TryGetValue(character, out bool canMove) && canMove;
    }

    private void ResetTurns()
    {
        foreach (var character in turns[currentTurn].Keys.ToList())
        {
            turns[currentTurn][character] = true;
        }
    }

    private bool HasAllMoved()
    {
        foreach (var canMove in turns[currentTurn].Values)
        {
            if (canMove) return false;
        }

        return true;
    }
}
