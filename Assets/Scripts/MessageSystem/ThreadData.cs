using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ThreadData : IChatData
{
    public List<MessageData> Messages { get; private set; }
    public List<IChatData> MessagesAsChatData => Messages.Select(x => x as IChatData).ToList();
    
    public string Name { get; }
    public Timestamp Timestamp { get; }
    public bool HasNewMessage { get; set; }
    public HighlightOnClick MessaageUIPrefab => MessageSystemManager.Instance.GetMessageUIPrefab(this);

    public event Action<ThreadData, MessageData> OnMessageAdded;

    public ThreadData(string name)
    {
        Messages = new List<MessageData>();
        Name = name;
        Timestamp = TimeSystem.Instance.GetCurrentTimestamp;
    }

    public void AddMessage(MessageData newMessage)
    {
        Messages.Add(newMessage);
        OnMessageAdded?.Invoke(this, newMessage);
        HasNewMessage = true;
    }
}
