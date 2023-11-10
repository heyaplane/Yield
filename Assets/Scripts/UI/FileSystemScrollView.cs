using System;
using UnityEngine;

public class FileSystemScrollView : MonoBehaviour
{
    [SerializeField] HighlightOnClick scrollViewItemPrefab;
    [SerializeField] Transform content;
    [SerializeField] Color itemHighlightColor;

    public HighlightOnClick CurrentlyHighlightedItem { get; private set; }
    
    public void PopulateView(string[] itemNames, Action<HighlightOnClick> onItemClick)
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
        CurrentlyHighlightedItem = null;
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateHighlightedItem(HighlightOnClick highlight)
    {
        if (highlight == CurrentlyHighlightedItem) return;
        
        if (CurrentlyHighlightedItem != null)
            CurrentlyHighlightedItem.Deselect();
        
        highlight.MarkAsSelected(itemHighlightColor);
        CurrentlyHighlightedItem = highlight;
    }

    public void ResetCurrentlySelected()
    {
        if (CurrentlyHighlightedItem != null)
            CurrentlyHighlightedItem.Deselect();
    }
}
