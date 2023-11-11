using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseScrollView : MonoBehaviour
{
    [SerializeField] RectTransform contentWindow;
    [SerializeField] RectTransform textTemplate;

    Dictionary<string, ScrollViewItem> itemLookup;
    
    public Action<string> OnScrollViewItemClickedEvent { get; set; }

    void Awake()
    {
        itemLookup = new Dictionary<string, ScrollViewItem>();
    }

    void OnEnable()
    {
        PopulateScrollView();
    }

    void PopulateScrollView()
    {
        ClearItems();

        string[] itemNames = GetItemNames();
        if (itemNames == null) return;
        
        foreach (string itemName in itemNames)
        {
            AddItemToScrollView(itemName);
        }
    }

    public void ClearItems()
    {
        foreach (string itemName in itemLookup.Keys)
        {
            DestroyItem(itemName);
        }
        
        itemLookup.Clear();
    }

    public void AddItemToScrollView(string itemName)
    {
        var buttonTransform = Instantiate(textTemplate, contentWindow);
        buttonTransform.gameObject.SetActive(true);
        var text = buttonTransform.GetComponentInChildren<TextMeshProUGUI>();
        text.text = itemName;
        var button = buttonTransform.GetComponent<Button>();
        button.onClick.AddListener(() => OnScrollViewItemClickedEvent?.Invoke(text.text));

        itemLookup[itemName] = new ScrollViewItem
        {
            Button = button,
            ButtonStaySelected = button.GetComponent<ButtonStaySelected>()
        };
    }

    public void RemoveScrollViewItem(string itemName)
    {
        DestroyItem(itemName);
        itemLookup.Remove(itemName);
    }

    public void DestroyItem(string itemName)
    {
        var item = itemLookup[itemName];
        item.Button.onClick.RemoveAllListeners();
        Destroy(item.Button.gameObject);
    }

    public void MarkItemAsSelected(string itemName)
    {
        foreach (var item in itemLookup)
        {
            if (item.Key == itemName)
                item.Value.ButtonStaySelected.SelectButton();
            else
                item.Value.ButtonStaySelected.DeselectButton();
        }
    }

    protected abstract string[] GetItemNames();
}

public struct ScrollViewItem
{
    public Button Button;
    public ButtonStaySelected ButtonStaySelected;
}
