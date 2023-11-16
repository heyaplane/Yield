using System;
using UnityEngine;

public class QuestManager : SingletonMonobehaviour<QuestManager>
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var thread = MessageSystemManager.Instance.AddThread("testThread");
            thread.AddMessage(new MessageData(new MessageSender("testSender", "Circle"), TimeSystem.Instance.GetCurrentTimestamp, "This is a test of the messaging system."));
        }
    }
}
