using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThreadSummaryUI : MonoBehaviour, IShowChatData
{
    [SerializeField] Image readStatusImage;
    [SerializeField] TextMeshProUGUI sendTimeText;
    [SerializeField] TextMeshProUGUI threadNameText;

    [SerializeField] HighlightOnClick highlightOnClick;
    public HighlightOnClick HighlightOnClick => highlightOnClick;

    public ThreadData Thread { get; private set; }

    public void InitializeMessageData(IChatData chatData)
    {
        if (chatData is not ThreadData threadData)
        {
            Debug.LogError("Tried passing non-ThreadData to ThreadSummaryUI.");
            return;
        }
        
        Thread = threadData;
        sendTimeText.text = threadData.Timestamp.GetFormattedTimestampText();
        threadNameText.text = threadData.Name;
    }
    
    public void ActionOnHighlight(HighlightOnClick obj)
    {
        if (!Thread.HasNewMessage) return;

        Thread.HasNewMessage = false;
        readStatusImage.gameObject.SetActive(false);
    }
}
