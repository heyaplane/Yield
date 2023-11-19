﻿using System;

[Serializable]
public class MessageData : IChatData
{
    public MessageSender MessageSender { get; }
    public Timestamp Timestamp { get; }
    public string MessageText { get; }
    public bool HasReply { get; set; }
    public HighlightOnClick MessaageUIPrefab => MessageSystemManager.Instance.GetMessageUIPrefab(this);

    public MessageData(MessageSender messageSender, Timestamp timestamp, string messageText)
    {
        MessageSender = messageSender;
        Timestamp = timestamp;
        MessageText = messageText;
    }
}
