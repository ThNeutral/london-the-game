using System.Collections.Generic;
using UnityEngine;

public enum Side
{
    Player,
    AI
}

public class TurnController : MonoBehaviour
{
    public class CharacterTurnData
    {
        public Side side;
        public bool available;

        public CharacterTurnData(Side side, bool available)
        {
            this.side = side;
            this.available = available;
        } 
    }

    [SerializeField]
    private Side initialTurn = Side.Player;
    private Side currentTurn;

    private readonly Dictionary<Character, Side> sides = new();
    private readonly Dictionary<Character, bool> turns = new();

    private void Start()
    {
        currentTurn = initialTurn;
    }

    public void Move(Character character)
    {
        Debug.Assert(CanMove(character), "Expected character to be able to move");
        turns[character] = true;

        if (IsEndOfTurn()) EndTurn();
    }

    public bool IsEndOfTurn()
    {
        foreach (var data in GetAllCharactersTurnData())
        {
            if (data.side == currentTurn && data.available)
            {
                return false;
            }
        }
        return true;
    }

    public void EndTurn()
    {
        foreach (var kvp in turns)
        {
            turns[kvp.Key] = true;
        }

        currentTurn = NextSide(currentTurn);
    }

    private Side NextSide(Side prev)
    {
        switch (prev)
        {
            case Side.Player:
                return Side.AI;

            case Side.AI:
                return Side.Player;

            default:
                Debug.LogError("how did you even got here????");
                return default;
        }
    }

    public bool CanMove(Character character)
    {
        var data = GetCharacterTurnData(character);
        if (data.side != currentTurn) return false;
        return data.available;
    }

    public CharacterTurnData GetCharacterTurnData(Character character)
    {
        Debug.Assert(sides.TryGetValue(character, out var side), "tried to access side of non-registered character");
        Debug.Assert(turns.TryGetValue(character, out var turn), "tried to access turn of non-registered character");

        return new(side, turn);
    }

    private CharacterTurnData[] GetAllCharactersTurnData()
    {
        Debug.Assert(sides.Count == turns.Count, "expected sides and count to have equal length");

        var datas = new List<CharacterTurnData>(sides.Count);
        foreach (var side in sides)
        {
            var turn = turns[side.Key];
            datas.Add(new CharacterTurnData(side.Value, turn));
        }

        return datas.ToArray();
    }
}