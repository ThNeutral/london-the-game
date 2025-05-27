using UnityEngine;

public class MapBusSubscriber : MonoBehaviour
{
    private MessageBus bus;

    [SerializeField]
    private MapEffectVisualizer visualizer;

    void Start()
    {
        bus = MessageBus.Instance;
        bus.Subscribe(MessageBus.EventType.CameraRaycastHitTilemapIdle, HandleCameraRaycastHitTilemapIdle);
    }

    void HandleCameraRaycastHitTilemapIdle(MessageBus.Event @event)
    {
        var hit = (RaycastHit)@event.payload;
        var cell = visualizer.WorldToCell(hit.point);
        visualizer.HighlightTiles(new Vector3Int[] { cell });
    }
}
