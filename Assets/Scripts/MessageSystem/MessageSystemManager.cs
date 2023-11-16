using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageSystemManager : SingletonMonobehaviour<MessageSystemManager>
{
    [SerializeField] Sprite aiIconSprite;
    [SerializeField] Sprite playerIconSprite;

    Dictionary<string, Sprite> iconSpriteLookup;
    public Sprite GetIconSprite(string iconSpriteID) => iconSpriteLookup[iconSpriteID];

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
}
