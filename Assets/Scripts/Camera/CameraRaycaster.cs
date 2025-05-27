using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private LayerMask mask;

    private void Update()
    {
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out var hitInfo, float.MaxValue, mask))
        {
            Debug.Log(hitInfo.point);
        }
    } 
}