using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MessageSystemManager : SingletonMonobehaviour<MessageSystemManager>
{
    [SerializeField] Sprite aiIconSprite;
    [SerializeField] Sprite playerIconSprite;

    Dictionary<string, Sprite> iconSpriteLookup;
    public Sprite GetIconSprite(string iconSpriteID) => iconSpriteLookup[iconSpriteID];

    [SerializeField] HighlightOnClick threadSummaryPrefab;
    [SerializeField] HighlightOnClick outsideMessagePrefab;
    [SerializeField] HighlightOnClick playerMessagePrefab;


    public HighlightOnClick GetMessageUIPrefab(IChatData chatData)
    {
        if (chatData is not MessageData messageData) return threadSummaryPrefab;
        return messageData.MessageSender.Name == "Player" ? playerMessagePrefab : outsideMessagePrefab;
    }

    public List<ThreadData> Threads { get; private set; }
    
    void OnEnable()
    {
        iconSpriteLookup = new Dictionary<string, Sprite>
        {
            {aiIconSprite.name, aiIconSprite},
            {playerIconSprite.name, playerIconSprite}
        };

        Threads = new List<ThreadData>();
    }

    public ThreadData AddThread(string threadName)
    {
        var newThread = new ThreadData(threadName);
        Threads.Add(newThread);
        EventManager.OnNewThreadAdded(newThread);
        return newThread;
    }

    public object CaptureSaveData()
    {
        return Threads.Select(x => x.GetSerializeableThread()).ToList();
    }

    public void RestoreSaveData(object saveData)
    {
        if (saveData is List<SerializableThread> serializableThreads)
        {
            foreach (var thread in serializableThreads)
            {
                Threads.Add(new ThreadData(thread));
            }
        }
    }
}
