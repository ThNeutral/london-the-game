using TMPro;
using UnityEngine;

public class TurnUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMesh;

    private MessageBus bus;

    private void Start()
    {
        bus = MessageBus.Instance;
        bus.Subscribe(MessageBus.EventType.RequestShowTurn, ShowTurn);
    }

    private void ShowTurn(MessageBus.Event @event)
    {
        var valid = @event.ReadPayload<string>(out var text);
        Debug.Assert(valid);

        textMesh.text = text;
    }
}