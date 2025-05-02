using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    [SerializeField]
    private InputActionReference rotateX;

    [SerializeField]
    private float rotationXSpeedInDegrees = 180;

    private float currentXRotation ;

    [SerializeField]
    private InputActionReference rotateY;
    
    [SerializeField]
    private float rotationYSpeedInDegrees = 45;

    [SerializeField]
    private float maxYRotationInDegrees = 75;

    [SerializeField]
    private float minYRotationInDegrees = 15;

    private float currentYRotation;

   
    [SerializeField]
    private new Camera camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentXRotation = camera.transform.eulerAngles.y;
        currentYRotation = camera.transform.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        var rotateXValue = rotateX.action.ReadValue<float>();
        currentXRotation += rotateXValue * rotationXSpeedInDegrees * Time.deltaTime;

        var rotateYValue = rotateY.action.ReadValue<float>();
        var newYRotation = currentYRotation + rotateYValue * rotationYSpeedInDegrees * Time.deltaTime;
        currentYRotation = Math.Clamp(newYRotation, minYRotationInDegrees, maxYRotationInDegrees);

        camera.transform.rotation = Quaternion.Euler(currentYRotation, currentXRotation, 0);
    }
}
