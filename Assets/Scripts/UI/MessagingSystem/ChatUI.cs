using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : BaseUI
{
    [SerializeField] SingleSelectMessageScrollView inboxScrollView;
    [SerializeField] SingleSelectMessageScrollView messagesScrollView;

    [SerializeField] Button closeUIButton;

    ThreadData currentMessagesThread;

    void OnEnable()
    {
        EventManager.OnNewThreadAddedEvent += HandleNewThreadAdded;
        
        closeUIButton.onClick.AddListener(HandleCloseUIButton);
    }

    void OnDisable()
    {
        EventManager.OnNewThreadAddedEvent -= HandleNewThreadAdded;
        
        closeUIButton.onClick.RemoveAllListeners();
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

    void HandleCloseUIButton()
    {
        CloseWindow();
        OnCancelAction?.Invoke();
    }
}
