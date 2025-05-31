using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MapBusSubscriber : MonoBehaviour
{
    private enum SelectionState
    {
        None,
        Move,
        Attack
    }

    private MessageBus bus;

    [SerializeField]
    private MapEffectVisualizer visualizer;

    private GridController gridController;

    private TurnController turnController;

    private Vector3Int startTile;
    private Vector3Int endMoveTile;

    private Vector3Int lookAtTile;
    private Vector3Int mouseAtTile;

    private SelectionState selectionState = SelectionState.None;

    private bool clickUnlocked = false;

    void Start()
    {
        bus = MessageBus.Instance;
        bus.Subscribe(MessageBus.EventType.CameraMove, HandleCameraMove);
        bus.Subscribe(MessageBus.EventType.CameraClick, HandleCameraClick);
        bus.Subscribe(MessageBus.EventType.ClickUnlock, HandleClickUnlock);
        bus.Subscribe(MessageBus.EventType.MouseMove, HandleMouseMove);

        gridController = GridController.Instance;
        turnController = TurnController.Instance;
    }

    private void HandleClickUnlock(MessageBus.Event @event)
    {
        if (!@event.ReadPayload<bool>(out var @new))
        {
            Debug.LogError("Unexpected invalid value in @event.ReadPayload");
            return;
        }

        clickUnlocked = @new;
    }

    private void HandleCameraMove(MessageBus.Event @event)
    {
        if (clickUnlocked) return;

        if (!@event.ReadPayload<RaycastHit>(out var hit))
        {
            Debug.LogError("Unexpected invalid value in @event.ReadPayload");
            return;
        }

        lookAtTile = visualizer.WorldToCell(hit.point);

        switch (selectionState)
        {
            case SelectionState.Move:
                {
                    var path = gridController.FindPath(startTile, lookAtTile);

                    visualizer.ResetTiles();
                    visualizer.MoveTiles(path);
                    visualizer.SelectTiles(new Vector3Int[] { startTile, lookAtTile });

                    break;
                }
        }
    }
    
    private void HandleMouseMove(MessageBus.Event @event)
    {
        if (!clickUnlocked) return;

        if (!@event.ReadPayload<RaycastHit>(out var hit))
        {
            Debug.LogError("Unexpected invalid value in @event.ReadPayload");
            return;
        }

        var newMouseAtTile = visualizer.WorldToCell(hit.point);

        switch (selectionState)
        {
            case SelectionState.Move:
                {
                    if (gridController.TryGetCharacter(startTile, out var character))
                    if (gridController.TryGetCharacter(newMouseAtTile, out var _)) return;

                    mouseAtTile = newMouseAtTile;
                    var path = gridController.FindPath(startTile, mouseAtTile, character);

                    visualizer.ResetTiles();
                    visualizer.MoveTiles(path);
                    visualizer.SelectTiles(new Vector3Int[] { startTile, mouseAtTile });

                    break;
                }
            case SelectionState.Attack: 
                {
                    if (gridController.TryGetCharacter(startTile, out var character))
                    if (gridController.TryGetCharacter(endMoveTile, out var _)) return;

                    mouseAtTile = newMouseAtTile;
                    var movePath = gridController.FindPath(startTile, endMoveTile, character);
                    var attackPath = gridController.FindPath(endMoveTile, mouseAtTile);

                    visualizer.ResetTiles();
                    visualizer.MoveTiles(movePath);
                    visualizer.AttackTiles(attackPath);
                    visualizer.SelectTiles(new Vector3Int[] { startTile, endMoveTile });

                    break;
                }
        }
    }

    private void HandleCameraClick(MessageBus.Event @event)
    {
        if (!@event.ReadPayload<RaycastHit>(out var hit))
        {
            Debug.LogError("Invalid event payload was passed.");
            return;
        }

        switch (selectionState)
        {
            case SelectionState.None:
                {
                    startTile = visualizer.WorldToCell(hit.point);
                    if (!gridController.TryGetCharacter(startTile, out var character)) return;
                    if (!turnController.CanMove(character)) return;

                    visualizer.ResetTiles();
                    visualizer.SelectTiles(new Vector3Int[] { startTile });

                    selectionState = SelectionState.Move;
                    break;
                }
            case SelectionState.Move:
                {
                    if (!gridController.TryGetCharacter(startTile, out var _)) return;

                    endMoveTile = visualizer.WorldToCell(hit.point);
                    if (gridController.TryGetCharacter(endMoveTile, out var _)) return;

                    selectionState = SelectionState.Attack;
                    break;
                }
            case SelectionState.Attack:
                {
                    visualizer.ResetTiles();
                    if (!gridController.TryGetCharacter(startTile, out var characterActor)) return;

                    var endAttackTile = visualizer.WorldToCell(hit.point);
                    if (!gridController.TryGetCharacter(endAttackTile, out var characterTarget)) return;

                    var actorTurnData = turnController.GetCharacterTurnData(characterActor);
                    var targetTurnData = turnController.GetCharacterTurnData(characterTarget);

                    var ability = characterActor.Abilities[0];
                    if (ability.CanInteractWithAlly && (actorTurnData.side == targetTurnData.side)) characterTarget.ReceiveDamage(ability.Damage);
                    if (ability.CanInteractWithEnemy && (actorTurnData.side != targetTurnData.side)) characterTarget.ReceiveDamage(ability.Damage);
                    
                    var path = gridController.FindPath(startTile, endMoveTile, characterActor);
                    if (!gridController.TryMoveCharacter(path)) return;
                    turnController.Move(characterActor);

                    selectionState = SelectionState.None;
                    break;
                }
        }
    }
}
