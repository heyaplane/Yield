using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour, IShowChatData
{
    [SerializeField] Image senderIconSprite;
    [SerializeField] TextMeshProUGUI sendTimeText;
    [SerializeField] TextMeshProUGUI senderNameText;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button replyButton;

    [SerializeField] HighlightOnClick highlightOnClick;
    public HighlightOnClick HighlightOnClick => highlightOnClick;

    MessageData message;

    void OnEnable()
    {
        highlightOnClick.OnClickedAction += ActionOnHighlight;
        if (replyButton != null)
            replyButton.onClick.AddListener(HandleReplyButton);
    }

    void OnDisable()
    {
        highlightOnClick.OnClickedAction -= ActionOnHighlight;
        
        if (replyButton != null)
            replyButton.onClick.RemoveAllListeners();
    }

    public void InitializeMessageData(IChatData chatData)
    {
        if (chatData is not MessageData messageData)
        {
            Debug.LogError("Tried passing non-MessageData to MessageUI");
            return;
        }
        
        message = messageData;
        senderIconSprite.sprite = messageData.MessageSender.IconSprite;
        sendTimeText.text = messageData.Timestamp.GetFormattedTimestampText();
        senderNameText.text = messageData.MessageSender.Name;
        messageText.text = messageData.MessageText;
    }

    public void ActionOnHighlight(HighlightOnClick obj)
    {
        if (message == null || message.HasReply || replyButton == null) return;
        
        replyButton.GetComponent<Image>().enabled = true;
        replyButton.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
    }

    public void AddListenerToReplyButton(UnityAction onReplyButton) => replyButton.onClick.AddListener(onReplyButton);

    void HandleReplyButton()
    {
        EventManager.OnReplyButtonClicked(message);
        EventManager.OnReportChosenEvent += HandleReportChosen;
    }

    void HandleReportChosen(VirtualReport reportFile, MessageData messageData)
    {
        EventManager.OnReportChosenEvent -= HandleReportChosen;
        if (reportFile == null) return;
        
        replyButton.GetComponent<Image>().enabled = false;
        replyButton.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        message.HasReply = true;
    }
}
