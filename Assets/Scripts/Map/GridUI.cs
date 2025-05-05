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
    private InputAction hover;

    public bool EnableGridUI { set => EnableGridUIInternal(value); }

    private void EnableGridUIInternal(bool value) {
        if (value)
        {
            click.Enable();
            hover.Enable();
        }
        else
        {
            click.Disable();
            hover.Disable();
        }
    }

    private void Awake()
    {
        controls = new GridControls();
    }

    private void OnEnable()
    {
        click = controls.Grid.Click;
        hover = controls.Grid.Hover;

        EnableGridUI = true;

        click.performed += HandleClick;
        hover.performed += HandleHover;
    }

    private void OnDisable()
    {
        click.performed -= HandleClick;
        hover.performed -= HandleHover;
        
        EnableGridUI = false;
    }

    private void HandleHover(InputAction.CallbackContext context)
    {
        if (ScreenRaycastToGridIndex(context.ReadValue<Vector2>(), out var tile)) 
        {
            gridController.HandleTileHover(tile);
        }
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        if (ScreenRaycastToGridIndex(context.ReadValue<Vector2>(), out var tile)) 
        {
            gridController.HandleTileClick(tile);
        }
    }

    private bool ScreenRaycastToGridIndex(Vector2 from, out Vector3Int tile)
    {
        if (from != Vector2.zero)
        {
            var ray = camera.ScreenPointToRay(from);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, tilemapLayerMask))
            {
                tile = gridController.WorldToGridIndex(hit.point);
                return true;
            }
        }

        tile = Vector3Int.zero;
        return false;
    }
}
