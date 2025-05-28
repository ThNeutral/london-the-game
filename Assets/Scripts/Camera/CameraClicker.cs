using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraClicker : MonoBehaviour
{
    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private CameraRaycaster raycaster;

    [SerializeField]
    private bool allowSelectTileNotUnderCamera = false;

    private CameraControls controls;

    private InputAction click;

    private MessageBus bus;

    private void Awake()
    {
        controls = new CameraControls();
    }

    private void Start()
    {
        bus = MessageBus.Instance;
    }

    private void OnEnable()
    {
        click = controls.Camera.Click;
        click.Enable();
        click.performed += OnClick;
    }

    private void OnDisable()
    {
        click.performed -= OnClick;
        click.Disable();
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        object payload = null;
        if (allowSelectTileNotUnderCamera)
        {
            var screenPoint = context.ReadValue<Vector2>();
            var ray = camera.ScreenPointToRay(screenPoint);
            if (!raycaster.Raycast(ray, out var hit)) return;
            payload = hit; 
        }
        bus.Publish(MessageBus.EventType.CameraClick, payload);
    }
}