using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GridUI : MonoBehaviour
{
    private enum SelectionState
    {
        NONE,
        ABILITY,
        POSITION,
        ATTACK,
    }

    [SerializeField] 
    private new Camera camera;

    [SerializeField]
    private GridController gridController;

    [SerializeField]
    private LayerMask tilemapLayerMask;

    private GridControls controls;
    private InputAction click;
    private InputAction hover;

    [SerializeField]
    private AbilityUI abilityUI;

    [SerializeField]
    private TurnController turnController;

    private SelectionState state;

    private Ability selectedAbility;

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
        if (state != SelectionState.POSITION && state != SelectionState.ATTACK) return;

        if (ScreenRaycastToGridIndex(context.ReadValue<Vector2>(), out var tile)) 
        {
            switch (state)
            {
                case SelectionState.POSITION:
                    {
                        gridController.UpdateMovementEndTile(tile);
                        break;
                    }
                case SelectionState.ATTACK:
                    {
                        gridController.UpdateAttackEndTile(tile);
                        break;
                    }
            }
        }
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        if (ScreenRaycastToGridIndex(context.ReadValue<Vector2>(), out var tile))
        {
            switch (state)
            {
                case SelectionState.NONE:
                    {
                        if (!gridController.GetCharacter(tile, out var character)) return;
                        if (!turnController.IsAllowedToMove(character)) return;
                        gridController.SelectInitialTile(tile);
                        abilityUI.PresentAbilityChoice(character, AbilityUICallback);
                        state = SelectionState.ABILITY;
                        break;
                    }
                case SelectionState.ABILITY:
                    {
                        break;
                    }
                case SelectionState.POSITION:
                    {
                        if (gridController.CommitMovement(selectedAbility))
                        {
                            if (selectedAbility.Type == AbilityType.MOVE)
                            {
                                state = SelectionState.NONE;
                                abilityUI.HideAbilityChoice();
                                selectedAbility = null;
                            }
                            else
                            {
                                state = SelectionState.ATTACK;
                            }
                        }
                        break;
                    }
                case SelectionState.ATTACK:
                    {
                        gridController.CommitAction(selectedAbility);
                        state  = SelectionState.NONE;
                        abilityUI.HideAbilityChoice();
                        selectedAbility = null;
                        break;
                    }
            }
        }
    }

    private void AbilityUICallback(Ability ability) 
    {
        if (selectedAbility != null) gridController.RollbackMovement();
        state = SelectionState.POSITION;
        gridController.ShowAbilityTiles(ability);
        selectedAbility = ability;
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
