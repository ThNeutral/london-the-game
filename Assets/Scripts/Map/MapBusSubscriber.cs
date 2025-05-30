using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapBusSubscriber : MonoBehaviour
{
    private enum SelectionState
    {
        None,
        SelectedTile,
    }

    private MessageBus bus;

    [SerializeField]
    private MapEffectVisualizer visualizer;

    private GridController gridController;

    private TurnController turnController;

    private Vector3Int currentSelectedTile;
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
            case SelectionState.SelectedTile:
                {
                    var path = gridController.FindPath(currentSelectedTile, lookAtTile);

                    visualizer.ResetTiles();
                    visualizer.HighlightTiles(path);
                    visualizer.SelectTiles(new Vector3Int[] { currentSelectedTile, lookAtTile });

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

        mouseAtTile = visualizer.WorldToCell(hit.point);

        switch (selectionState)
        {
            case SelectionState.SelectedTile:
                {
                    var path = gridController.FindPath(currentSelectedTile, mouseAtTile);

                    visualizer.ResetTiles();
                    visualizer.HighlightTiles(path);
                    visualizer.SelectTiles(new Vector3Int[] { currentSelectedTile, mouseAtTile });

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

                    currentSelectedTile = visualizer.WorldToCell(hit.point);
                    if (!gridController.TryGetCharacter(currentSelectedTile, out var character)) return;
                    if (!turnController.CanMove(character)) return;

                    visualizer.ResetTiles();
                    visualizer.SelectTiles(new Vector3Int[] { currentSelectedTile });

                    selectionState = SelectionState.SelectedTile;
                    break;
                }
            case SelectionState.SelectedTile:
                {
                    visualizer.ResetTiles();
                    if (!gridController.TryGetCharacter(currentSelectedTile, out var character)) return;
                    if (!gridController.TryMoveCharacter(currentSelectedTile, visualizer.WorldToCell(hit.point))) return;

                    turnController.Move(character);

                    selectionState = SelectionState.None;
                    break;
                }
        }
    }
}
