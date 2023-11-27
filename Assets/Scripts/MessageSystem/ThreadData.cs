using System;
using System.Collections.Generic;
using System.Linq;

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

    public ThreadData(SerializableThread serializableThread)
    {
        Name = serializableThread.Name;
        Timestamp = serializableThread.Timestamp;
        HasNewMessage = serializableThread.HasNewMessage;
        Messages = serializableThread.Messages;
    }

    public void AddMessage(MessageData newMessage)
    {
        Messages.Add(newMessage);
        OnMessageAdded?.Invoke(this, newMessage);
        HasNewMessage = true;
    }

    public SerializableThread GetSerializeableThread()
    {
        return new SerializableThread
        {
            Name = Name,
            Timestamp = Timestamp,
            HasNewMessage = HasNewMessage,
            Messages = Messages
        };
    }
}

[Serializable]
public class SerializableThread
{
    public string Name;
    public Timestamp Timestamp;
    public bool HasNewMessage;
    public List<MessageData> Messages;
}
