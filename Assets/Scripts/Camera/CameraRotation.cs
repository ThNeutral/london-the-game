using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    [SerializeField]
    private float rotationXStepInDegrees = 90;

    private float targetXRotation;

    private Vector3 target;
    public Vector3 Target { set => SetTarget(value); }
    private void SetTarget(Vector3 value) {
        target = value;
        transform.position = value;
    }

    [SerializeField]
    private float rotationXSmoothness = 5;

    [SerializeField]
    private float rotationYSpeedInDegrees = 45;

    [SerializeField]
    private float maxYRotationInDegrees = 60;

    [SerializeField]
    private float minYRotationInDegrees = 60;

    private float currentYRotation;

    [SerializeField]
    private new Camera camera;

    private CameraControls controls;

    private InputAction rotateX;
    private InputAction rotateY;

    private void Awake()
    {
        controls = new CameraControls();
    }

    private void OnEnable()
    {
        rotateX = controls.Camera.RotateAlongX;
        rotateX.Enable();
        rotateX.performed += OnRotateX;

        rotateY = controls.Camera.RotateAlongY;
        rotateY.Enable();
    }

    private void OnDisable()
    {
        rotateX.performed -= OnRotateX;
        rotateX.Disable();

        rotateY.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetXRotation = camera.transform.eulerAngles.y;
        currentYRotation = Math.Clamp(
            camera.transform.eulerAngles.x,
            minYRotationInDegrees,
            maxYRotationInDegrees
        );
    }

    // Update is called once per frame
    void Update()
    {
        var rotateYValue = rotateY.ReadValue<float>();
        var newYRotation = currentYRotation + rotateYValue * rotationYSpeedInDegrees * Time.deltaTime;
        currentYRotation = Math.Clamp(newYRotation, minYRotationInDegrees, maxYRotationInDegrees);

        var currentY = transform.rotation.eulerAngles.y;
        var delta = Mathf.DeltaAngle(currentY, targetXRotation);
        var xRotation = currentY + delta * rotationXSmoothness * Time.deltaTime;

        transform.rotation = Quaternion.Euler(currentYRotation, xRotation, 0);
    }

    private void OnRotateX(InputAction.CallbackContext context)
    {
        var rotateXValue = context.ReadValue<float>();
        targetXRotation += rotateXValue * rotationXStepInDegrees;
    }
}