using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Side
{
    Player,
    AI
}

public class TurnController : MonoBehaviour
{
    private static TurnController _instance;
    public static TurnController Instance
    {
        get
        {
            Debug.Assert(_instance != null, "Turn Controller was not created in the scene.");
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("Multiple Turn Controller instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
    }

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

    public void AddCharacter(Character character, Side side, bool available)
    {
        Debug.Assert(!sides.ContainsKey(character));
        Debug.Assert(!turns.ContainsKey(character));

        sides.Add(character, side);
        turns.Add(character, available);
    }

    private void Start()
    {
        currentTurn = initialTurn;
    }

    public void Move(Character character)
    {
        Debug.Assert(CanMove(character), "Expected character to be able to move");
        turns[character] = false;

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
        foreach (var key in turns.Keys.ToList())
        {
            turns[key] = true;
        }

        NextSide();
    }

    private void NextSide()
    {
        switch (currentTurn)
        {
            case Side.Player:
                currentTurn = Side.AI;
                break;
            case Side.AI:
                currentTurn = Side.Player;
                break;
            default:
                Debug.LogError("how did you even got here????");
                break;
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