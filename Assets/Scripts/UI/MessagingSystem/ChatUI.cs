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
        closeUIButton.onClick.AddListener(HandleCloseUIButton);

        if (MessageSystemManager.Instance == null) return;
        
    }

    void OnDisable()
    {
        closeUIButton.onClick.RemoveAllListeners();
    }

    public override void EnableWindow()
    {
        base.EnableWindow();
        
        inboxScrollView.ClearView();
        foreach (var thread in MessageSystemManager.Instance.Threads)
        {
            StartCoroutine(inboxScrollView.AddItemToView(thread, PopulateMessageScrollView));
        }
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
        messagesScrollView.ScrollToBottom();
    }

    void UpdateMessageScrollView(ThreadData thread, MessageData messageData)
    {
        messagesScrollView.ClearView();
        messagesScrollView.AddItemsToView(thread.MessagesAsChatData, null);
        messagesScrollView.ScrollToBottom();
    }

    void HandleCloseUIButton()
    {
        CloseWindow();
        OnCancelAction?.Invoke();
    }
}
