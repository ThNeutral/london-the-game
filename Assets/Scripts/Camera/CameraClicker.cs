using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraClicker : MonoBehaviour
{
    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private CameraRaycaster raycaster;

    private bool? prevAllowSelectTileNotUnderCamera = null;
    [SerializeField]
    private bool allowSelectTileNotUnderCamera = false;
    public bool AllowSelectTileNotUnderCamera
    {
        get => allowSelectTileNotUnderCamera;
    }

    private CameraControls controls;

    private InputAction click;
    private InputAction move;

    private MessageBus bus;

    private void Awake()
    {
        controls = new CameraControls();
    }

    private void OnEnable()
    {
        click = controls.Camera.Click;
        click.Enable();
        click.performed += OnClick;

        move = controls.Camera.MoveMouse;
        move.Enable();
        move.performed += OnMove;
    }

    private void OnDisable()
    {
        click.performed -= OnClick;
        click.Disable();

        move.performed -= OnMove;
        move.Disable();
    }

    private void Start()
    {
        bus = MessageBus.Instance;
    }

    private void Update()
    {
        if (prevAllowSelectTileNotUnderCamera != allowSelectTileNotUnderCamera)
        {
            prevAllowSelectTileNotUnderCamera = allowSelectTileNotUnderCamera;
            bus.Publish(MessageBus.EventType.ClickUnlock, allowSelectTileNotUnderCamera);
        }
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Ray ray;
        if (allowSelectTileNotUnderCamera)
        {
            var screenPoint = context.ReadValue<Vector2>();
            ray = camera.ScreenPointToRay(screenPoint);
        }
        else
        {
            ray = new Ray(camera.transform.position, camera.transform.forward);
        }
        if (!raycaster.Raycast(ray, out var hit)) return;
        bus.Publish(MessageBus.EventType.CameraClick, hit);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        var screenPoint = context.ReadValue<Vector2>();
        var ray = camera.ScreenPointToRay(screenPoint);
        if (!raycaster.Raycast(ray, out var hit)) return;
        bus.Publish(MessageBus.EventType.MouseMove, hit);
    }
}