using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private LayerMask mask;

    private MessageBus messageBus;

    private Vector3 prevPosition;
    private Vector3 prevForward;

    void Start()
    {
        messageBus = MessageBus.Instance;
    }

    private void Update()
    {
        if ((camera.transform.position != prevPosition || camera.transform.forward != prevForward) &&
            Physics.Raycast(camera.transform.position, camera.transform.forward, out var hitInfo, float.MaxValue, mask)
        )
        {
            messageBus.Publish(MessageBus.EventType.CameraRaycastHitTilemapIdle, hitInfo);

            prevPosition = camera.transform.position;
            prevForward = camera.transform.forward;
        }
    } 
}