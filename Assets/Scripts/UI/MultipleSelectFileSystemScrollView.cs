using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultipleSelectFileSystemScrollView : MonoBehaviour
{
    [SerializeField] HighlightOnClick scrollViewItemPrefab;
    [SerializeField] Transform content;
    [SerializeField] Color itemHighlightColor;

    public List<HighlightOnClick> CurrentlyHighlightedItems { get; private set; }
    public string[] CurrentlyHighlightedFileNames => CurrentlyHighlightedItems.Select(x => x.ItemString).ToArray();

    void Awake()
    {
        CurrentlyHighlightedItems = new List<HighlightOnClick>();
    }

    public void AddItemsToView(string[] itemNames, Action<HighlightOnClick> onItemClick)
    {
        foreach (string itemName in itemNames)
        {
            var highlight = Instantiate(scrollViewItemPrefab, content);
            highlight.Text.text = itemName;
            if (onItemClick != null)
                highlight.OnClickedAction += onItemClick;
            highlight.OnClickedAction += UpdateHighlightedItem;
            highlight.gameObject.SetActive(true);
        }
    }

    public void ClearView()
    {
        CurrentlyHighlightedItems.Clear();
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateHighlightedItem(HighlightOnClick highlight)
    {
        if (CurrentlyHighlightedItems.Contains(highlight)) return;
        
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            ResetCurrentlySelected();
        
        highlight.MarkAsSelected(itemHighlightColor);
        CurrentlyHighlightedItems.Add(highlight);
    }

    public void ResetCurrentlySelected()
    {
        foreach (var highlightedItem in CurrentlyHighlightedItems)
        {
            highlightedItem.Deselect();
        }
    
        CurrentlyHighlightedItems.Clear();
    }
}
