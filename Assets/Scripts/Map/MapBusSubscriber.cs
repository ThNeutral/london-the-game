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
    }

    void HandleCameraRaycastHitTilemapIdle(MessageBus.Event @event)
    {
        var hit = (RaycastHit)@event.payload;
        currentSelectedTile = visualizer.WorldToCell(hit.point);
        visualizer.ResetTiles();
        visualizer.HighlightTiles(new Vector3Int[] { currentSelectedTile });
    }
}
