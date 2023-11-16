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
    }

    void OnDisable()
    {
        highlightOnClick.OnClickedAction -= ActionOnHighlight;
        
        replyButton.onClick.RemoveAllListeners();
    }

    public void InitializeMessageData(IChatData chatData)
    {
        if (chatData is not MessageData messageData)
        {
            Debug.LogError("Tried passing non-MessageData to MessageUI");
            return;
        }
        
        this.message = messageData;
        senderIconSprite.sprite = messageData.MessageSender.IconSprite;
        sendTimeText.text = messageData.Timestamp.GetFormattedTimestampText();
        senderNameText.text = messageData.MessageSender.Name;
        messageText.text = messageData.MessageText;
    }

    public void ActionOnHighlight(HighlightOnClick obj)
    {
        if (message.HasReply) return;
        
        replyButton.GetComponent<Image>().enabled = true;
        replyButton.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
    }

    public void AddListenerToReplyButton(UnityAction onReplyButton) => replyButton.onClick.AddListener(onReplyButton);
}
