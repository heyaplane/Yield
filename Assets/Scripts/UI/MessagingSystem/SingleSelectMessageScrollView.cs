using System;
using System.Collections.Generic;
using UnityEngine;

public class SingleSelectMessageScrollView : MonoBehaviour
{
    [SerializeField] HighlightOnClick scrollViewItemPrefab;
    [SerializeField] Transform content;
    [SerializeField] Color itemHighlightColor;

    public HighlightOnClick CurrentlyHighlightedItem { get; private set; }
    
    public void AddItemsToView(List<IChatData> messageDatas, Action<HighlightOnClick> onItemClick)
    {
        foreach (var messageData in messageDatas)
        {
            AddItemToView(messageData, onItemClick);
        }
    }

    public void AddItemToView(IChatData messageData, Action<HighlightOnClick> onItemClick)
    {
        var highlight = Instantiate(scrollViewItemPrefab, content);
        var showData = highlight.GetComponent<IShowChatData>();
        showData.InitializeMessageData(messageData);
        if (onItemClick != null)
            highlight.OnClickedAction += onItemClick;
        highlight.OnClickedAction += UpdateHighlightedItem;
        highlight.gameObject.SetActive(true);
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
    
    public List<string> GetAllItemNames()
    {
        var names = new List<string>();
        foreach (Transform child in content)
        {
            var highlight = child.gameObject.GetComponent<HighlightOnClick>();
            names.Add(highlight.ItemString);
        }

        return names;
    }
}
