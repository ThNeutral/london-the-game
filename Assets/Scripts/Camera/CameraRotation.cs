using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    [SerializeField]
    private float rotationXSpeedInDegrees = 180;

    private float currentXRotation ;
    
    [SerializeField]
    private float rotationYSpeedInDegrees = 45;

    [SerializeField]
    private float maxYRotationInDegrees = 75;

    [SerializeField]
    private float minYRotationInDegrees = 15;

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

        rotateY = controls.Camera.RotateAlongY;
        rotateY.Enable();
    }

    private void OnDisable()
    {
        rotateX.Disable();
        rotateY.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentXRotation = camera.transform.eulerAngles.y;
        currentYRotation = Math.Clamp(
            camera.transform.eulerAngles.x,
            minYRotationInDegrees,
            maxYRotationInDegrees
        );
    }

    // Update is called once per frame
    void Update()
    {
        var rotateXValue = rotateX.ReadValue<float>();
        currentXRotation += rotateXValue * rotationXSpeedInDegrees * Time.deltaTime;

        var rotateYValue = rotateY.ReadValue<float>();
        var newYRotation = currentYRotation + rotateYValue * rotationYSpeedInDegrees * Time.deltaTime;
        currentYRotation = Math.Clamp(newYRotation, minYRotationInDegrees, maxYRotationInDegrees);

        camera.transform.rotation = Quaternion.Euler(currentYRotation, currentXRotation, 0);
    }
}
