using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    public class AbilityUIShowRequest
    {
        public string Label;
        public Action Callback;
    }

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private int initialNumberOfButtons;
    private int numberOfActiveButtons;

    [SerializeField]
    private Vector2 startPosition = new(-600, -150);
    [SerializeField]
    private Vector2 offset = new(0, 50);

    private List<Button> buttons;
    private List<TextMeshProUGUI> textMeshes;

    private List<int> highlightedIndexes;
    private List<Image> images;

    private MessageBus bus;

    void Start()
    {
        buttons = new List<Button>(initialNumberOfButtons);
        textMeshes = new List<TextMeshProUGUI>(initialNumberOfButtons);
        images = new List<Image>(initialNumberOfButtons);
        highlightedIndexes = new();
        CheckNumberOfButtons(initialNumberOfButtons);

        bus = MessageBus.Instance;
        bus.Subscribe(MessageBus.EventType.RequestShowSelectAbilities, ShowSelectAbilities);
        bus.Subscribe(MessageBus.EventType.RequestHideSelectAbilities, HideSelectAbilities);
        bus.Subscribe(MessageBus.EventType.RequestHighlightSelectAbilities, HighlightSelectAbility);
        bus.Subscribe(MessageBus.EventType.RequestClearHighlightSelectAbilities, ClearHighlightSelectAbility);
    }

    void ShowSelectAbilities(MessageBus.Event @event)
    {
        Debug.Assert(numberOfActiveButtons == 0);
        var valid = @event.ReadPayload<List<AbilityUIShowRequest>>(out var showRequests);
        Debug.Assert(valid);
        CheckNumberOfButtons(showRequests.Count);

        numberOfActiveButtons = showRequests.Count;
        for (int i = 0; i < showRequests.Count; i++)
        {
            var request = showRequests[i];
            var button = buttons[i];
            var text = textMeshes[i];

            button.gameObject.SetActive(true);
            button.onClick.AddListener(() => request.Callback());
            text.text = request.Label;
        }
    }

    void HighlightSelectAbility(MessageBus.Event @event)
    {
        var valid = @event.ReadPayload<int>(out var index);
        Debug.Assert(valid);
        Debug.Assert(!highlightedIndexes.Contains(index));
        Debug.Assert(index < images.Count);

        highlightedIndexes.Add(index);
        images[index].enabled = true;
    }

    void ClearHighlightSelectAbility(MessageBus.Event @event)
    {
        foreach (var index in highlightedIndexes)
        {
            images[index].enabled = false;
        }
        highlightedIndexes.Clear();
    }

    void HideSelectAbilities(MessageBus.Event @event)
    {
        Debug.Assert(buttons.Count >= numberOfActiveButtons);

        for (int i = 0; i < numberOfActiveButtons; i++)
        {
            var button = buttons[i];
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }
    } 
    
    private void CheckNumberOfButtons(int targetSize)
    {
        var currentCount = buttons.Count;
        for (int i = 0; i < targetSize - currentCount; i++)
        {
            var button = Instantiate(buttonPrefab, canvas.transform).GetComponent<Button>();
            Debug.Assert(button != null, "Button prefab does not contain Button component");
            buttons.Add(button);
            button.gameObject.SetActive(false);

            var textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Assert(textMesh != null, "Button prefab does not contain TextMesh component in its children");
            textMeshes.Add(textMesh);

            var image = button.GetComponent<Image>();
            Debug.Assert(image != null, "Button prefab must contain Image component");
            images.Add(image);
            image.enabled = false;

            int index = currentCount + i;
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            Vector2 position = startPosition + index * offset;
            rectTransform.anchoredPosition = position;
        }
    }
}