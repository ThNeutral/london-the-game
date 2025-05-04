using UnityEngine;
using UnityEngine.InputSystem;

public class GridUI : MonoBehaviour
{
    [SerializeField] 
    private new Camera camera;

    [SerializeField]
    private GridController gridController;

    [SerializeField]
    private LayerMask tilemapLayerMask;

    private GridControls controls;
    private InputAction click;

    private void Awake()
    {
        controls = new GridControls();
    }

    private void OnEnable()
    {
        click = controls.Grid.Click;
        click.Enable();
        click.performed += HandleClick;
    }

    private void OnDisable()
    {
        click.performed -= HandleClick;
        click.Disable();
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        var mousePosition = context.ReadValue<Vector2>();
        if (mousePosition != Vector2.zero)
        {
            var ray = camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, tilemapLayerMask))
            {
                var tilePos = gridController.WorldToGridIndex(hit.point);
                gridController.HandleTileClick(tilePos);
            }
        }
    }
}
