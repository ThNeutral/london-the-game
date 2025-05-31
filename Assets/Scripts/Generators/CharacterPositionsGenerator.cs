using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CharacterPositionGenerationData
{
    public Vector2Int tile;
    public GameObject prefab;
    public Side side;
}

public class CharacterPositionsGenerator : MonoBehaviour
{
    [SerializeField]
    private MapEffectVisualizer visualizer;

    [SerializeField]
    private List<CharacterPositionGenerationData> generationDatas;

    private TurnController turnController;
    private GridController gridController;

    private void Start()
    {
        turnController = TurnController.Instance;
        gridController = GridController.Instance;

        GenerateCharacterPositions();
    }

    private void GenerateCharacterPositions()
    {
        foreach (var data in generationDatas)
        {
            var character = Instantiate(data.prefab).GetComponent<Character>();

            var v3int = (Vector3Int)data.tile;
            var world = visualizer.CellToWorldCentered(v3int);
            world.y += 1f;
            character.transform.position = world;

            gridController.AddCharacter(v3int, character);
            turnController.AddCharacter(character, data.side, true);
        }
    }
}