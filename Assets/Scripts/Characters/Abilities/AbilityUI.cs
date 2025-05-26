using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using TMPro;

public class AbilityUI : MonoBehaviour 
{
    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private Vector3 initialPosition = new (-650, -300, 0);

    [SerializeField]
    private Vector3 offset = new (0, 60, 0);

    private List<Button> buttons = new();

    void Start()
    {
        InstantiateButtons(5);
    }
    
    public void PresentAbilityChoice(Character character, Action<Ability> callback)
    {
        var abilities = character.Abilities;
     
        var sizeDiff = abilities.Length - buttons.Count;
        if (sizeDiff > 0)
        {
            InstantiateButtons(sizeDiff);
        }
        
        for (int i = 0; i < abilities.Length; i++)
        {
            var capturedIndex = i;
            var button = buttons[capturedIndex];
            button.gameObject.SetActive(true);
            button.onClick.AddListener(() => callback.Invoke(abilities[capturedIndex]));
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"A{i}";
        }
    }

    public void HideAbilityChoice()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }

    private void InstantiateButtons(int size, bool active = false)
    {
        int startIndex = buttons.Count;

        for (int i = 0; i < size; i++)
        {
            var button = Instantiate(buttonPrefab, transform);
            var position = initialPosition + offset * (startIndex + i);
            (button.transform as RectTransform).anchoredPosition = position;
            buttons.Add(button.GetComponent<Button>());
            button.SetActive(active);
        }
    }
}