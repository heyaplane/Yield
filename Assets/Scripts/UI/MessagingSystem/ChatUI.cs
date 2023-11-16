using System;
using System.Linq;
using UnityEngine;

public class ChatUI : BaseUI
{
    [SerializeField] SingleSelectMessageScrollView inboxScrollView;
    [SerializeField] SingleSelectMessageScrollView messagesScrollView;

    void OnEnable()
    {
        EventManager.OnNewThreadAddedEvent += HandleNewThreadAdded;
    }

    void OnDisable()
    {
        EventManager.OnNewThreadAddedEvent -= HandleNewThreadAdded;
    }

    void HandleNewThreadAdded(ThreadData newThread)
    {
        inboxScrollView.AddItemToView(newThread, PopulateMessageScrollView);
    }

    void PopulateMessageScrollView(HighlightOnClick highlightOnClick)
    {
        var thread = highlightOnClick.GetComponent<ThreadSummaryUI>().Thread;
        messagesScrollView.AddItemsToView(thread.MessagesAsChatData, null);
    }
}
