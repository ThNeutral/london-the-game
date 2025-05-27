using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float cameraMoveRate = 5;

    [SerializeField]
    private new Camera camera;    

    [SerializeField]
    private Vector2 moveXLimit;
    [SerializeField]
    private Vector2 moveZLimit;

    private CameraControls controls;

    private InputAction cameraMove;

    private void Awake()
    {
        controls = new CameraControls();
    }

    private void OnEnable()
    {
        cameraMove = controls.Camera.MoveCamera;
        cameraMove.Enable();
    }

    private void OnDisable()
    {
        cameraMove.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var moveInput = cameraMove.ReadValue<Vector2>();
        var moveRotation = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0);
        var moveDifference = new Vector3(-moveInput.x, 0, -moveInput.y);
        var newTransform = transform.position + moveRotation * moveDifference * cameraMoveRate * Time.deltaTime;
        transform.position = new Vector3(
            Mathf.Clamp(newTransform.x, moveXLimit.x, moveXLimit.y),
            newTransform.y,
            Mathf.Clamp(newTransform.z, moveZLimit.x, moveZLimit.y)
        );
    }
}