using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    [SerializeField]
    private float cameraZoomRate = 5;

    [SerializeField]
    private float zoomSmoothness = 5;

    [SerializeField]
    private float minZoom = 3;
    [SerializeField]
    private float maxZoom = 10;

    private float currentZoom;

    [SerializeField]
    private new Camera camera;

    private CameraControls controls;

    private InputAction cameraZoom;

    private void Awake()
    {
        controls = new CameraControls();
    }

    private void OnEnable()
    {
        cameraZoom = controls.Camera.CameraZoom;
        cameraZoom.Enable();
    }

    private void OnDisable()
    {
        cameraZoom.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentZoom = camera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        var mouseScroll = cameraZoom.ReadValue<float>();
        currentZoom = Mathf.Clamp(currentZoom - mouseScroll * cameraZoomRate * Time.deltaTime, minZoom, maxZoom);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, currentZoom, zoomSmoothness * Time.deltaTime);
    }
}
