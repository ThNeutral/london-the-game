using System.Collections.Generic;
using UnityEngine;

public class MapBusSubscriber : MonoBehaviour
{
    private MessageBus bus;

    [SerializeField]
    private MapEffectVisualizer visualizer;

    private Vector3Int currentSelectedTile;

    void Start()
    {
        bus = MessageBus.Instance;
        bus.Subscribe(MessageBus.EventType.CameraRaycastHitTilemapIdle, HandleCameraRaycastHitTilemapIdle);
        bus.Subscribe(MessageBus.EventType.CameraClick, HandleCameraClick);
    }

    void HandleCameraRaycastHitTilemapIdle(MessageBus.Event @event)
    {
        if (!@event.ReadPayload<RaycastHit>(out var hit))
        {
            Debug.LogError("Unexpected invalid value in @event.ReadPayload");
            return;
        }

        var newSelectedTile = visualizer.WorldToCell(hit.point);
        if (currentSelectedTile == newSelectedTile) return;

        currentSelectedTile = newSelectedTile;
        visualizer.ResetTiles();
        visualizer.HighlightTiles(new Vector3Int[] { currentSelectedTile });
    }

    void HandleCameraClick(MessageBus.Event @event)
    {
        var tiles = new List<Vector3Int>();
        if (@event.IsPayloadNull())
        {
            // If payload is null, select tile under camera
            tiles.Add(currentSelectedTile);
        }
        else if (@event.ReadPayload<RaycastHit>(out var hit))
        {
            // If payload is RaycastHit, select tile in this place
            var tilePosition = visualizer.WorldToCell(hit.point);
            tiles.Add(tilePosition);
        } else
        {
            Debug.LogError("Invalid event payload was passed.");
            return;
        }

        visualizer.ResetTiles();
        visualizer.SelectTiles(tiles.ToArray());
    }
}
