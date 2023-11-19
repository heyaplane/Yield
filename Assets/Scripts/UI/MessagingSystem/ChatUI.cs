using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChatUI : BaseUI
{
    [SerializeField] SingleSelectMessageScrollView inboxScrollView;
    [SerializeField] SingleSelectMessageScrollView messagesScrollView;

    ThreadData currentMessagesThread;

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
        StartCoroutine(inboxScrollView.AddItemToView(newThread, PopulateMessageScrollView));
    }

    void PopulateMessageScrollView(HighlightOnClick highlightOnClick)
    {
        var newThread = highlightOnClick.GetComponent<ThreadSummaryUI>().Thread;
        if (currentMessagesThread == newThread) return;
        
        messagesScrollView.ClearView();
        
        if (currentMessagesThread != null)
            currentMessagesThread.OnMessageAdded -= UpdateMessageScrollView;

        if (newThread == null) return;
        
        newThread.OnMessageAdded += UpdateMessageScrollView;
        currentMessagesThread = newThread;
        messagesScrollView.AddItemsToView(currentMessagesThread.MessagesAsChatData, null);
    }

    void UpdateMessageScrollView(ThreadData thread, MessageData messageData)
    {
        messagesScrollView.ClearView();
        messagesScrollView.AddItemsToView(thread.MessagesAsChatData, null);
    }
}
