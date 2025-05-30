using UnityEngine;
using System.Collections.Generic;

public class CharacterPositionsGenerator : MonoBehaviour
{
    [SerializeField]
    private MapEffectVisualizer visualizer;

    [SerializeField]
    private GameObject allyPrefab;

    [SerializeField]
    private GameObject enemyPrefab;

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
        var allyTile = new Vector3Int(-2, -2);
        var allyCharacter = Instantiate(allyPrefab).GetComponent<Character>();

        var allyWorld = visualizer.CellToWorldCentered(allyTile);
        allyWorld.y += 1f;
        allyCharacter.transform.position = allyWorld;

        gridController.AddCharacter(allyTile, allyCharacter);
        turnController.AddCharacter(allyCharacter, Side.Player, true);

        var enemyTile = new Vector3Int(2, 2);
        var enemyCharacter = Instantiate(enemyPrefab).GetComponent<Character>();

        var enemyWorld = visualizer.CellToWorldCentered(enemyTile);
        enemyWorld.y += 1f;
        enemyCharacter.transform.position = enemyWorld;

        gridController.AddCharacter(enemyTile, enemyCharacter);
        turnController.AddCharacter(enemyCharacter, Side.AI, true);
    }
}