using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSelectMessageScrollView : MonoBehaviour
{
    [SerializeField] Transform content;
    [SerializeField] Color itemHighlightColor;

    public HighlightOnClick CurrentlyHighlightedItem { get; private set; }
    
    public void AddItemsToView(List<IChatData> chatDatas, Action<HighlightOnClick> onItemClick)
    {
        foreach (var chatData in chatDatas)
        {
            StartCoroutine(AddItemToView(chatData, onItemClick));
        }
    }

    public IEnumerator AddItemToView(IChatData chatData, Action<HighlightOnClick> onItemClick)
    {
        var scrollViewItemPrefab = chatData.MessaageUIPrefab;
        var highlight = Instantiate(scrollViewItemPrefab, content);
        var showData = highlight.GetComponent<IShowChatData>();
        showData.InitializeMessageData(chatData);
        if (onItemClick != null)
            highlight.OnClickedAction += onItemClick;
        highlight.OnClickedAction += UpdateHighlightedItem;

        yield return null;
        yield return null;
        
        highlight.GetComponent<CanvasGroup>().alpha = 1;
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
