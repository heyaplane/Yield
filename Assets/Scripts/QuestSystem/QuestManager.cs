using System;
using UnityEngine;

public class QuestManager : SingletonMonobehaviour<QuestManager>
{
    ThreadData thread;
    MessageSender outsideSender;

    [SerializeField] ErrorEvolver errorEvolver;
    [SerializeField] QuestSO startingQuest;

    void OnEnable()
    {
        outsideSender = new MessageSender("Outside", "Circle");
        
        EventManager.OnReportChosenEvent += HandleReportSubmitted;
    }

    void OnDisable()
    {
        EventManager.OnReportChosenEvent -= HandleReportSubmitted;
    }

    void Start()
    {
        WaferManager.Instance.ActiveWafer = startingQuest.StartingWafer;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            thread = MessageSystemManager.Instance.AddThread("testThread");
            SendShortMessage(outsideSender);
        }
        
        else if (Input.GetKeyDown(KeyCode.M))
        {
            SendLongMessage(outsideSender);
        }
    }

    public void SendShortMessage(MessageSender sender)
    {
        thread.AddMessage(new MessageData(sender, TimeSystem.Instance.GetCurrentTimestamp, "This is a test of the messaging system."));
    }

    public void SendReportMessage(MessageSender sender, VirtualReport reportFile)
    {
        thread.AddMessage(new MessageData(sender, TimeSystem.Instance.GetCurrentTimestamp, $"Here's my report for the measured samples:\n{reportFile.FileName}"));
    }

    public void SendLongMessage(MessageSender sender)
    {
        thread.AddMessage(new MessageData(sender, TimeSystem.Instance.GetCurrentTimestamp, 
            "This is another test of the messaging system.\n" +
            "This time, I'm testing what a paragraph looks like in the message log.\n\n" + 
            "This is the effect of multiple new lines and lots and lots and lots and lots and lots and lots and lots and lots and lots and lots and lots and lots of text.\n\n" +
            "Hopefully this does a good job illustrating what lots and lots of text looks like in game."));        
    }

    void HandleReportSubmitted(VirtualReport reportFile, MessageData messageData)
    {
        if (reportFile == null || messageData == null) return;
        SendReportMessage(new MessageSender("Player", "Square"), reportFile);

        var nextWaferEvolution = errorEvolver.GetNextWaferEvolution(startingQuest, reportFile.WaferData, reportFile.ProcessRecommendation);
        WaferManager.Instance.ActiveWafer = nextWaferEvolution;
        thread.AddMessage(new MessageData(outsideSender, TimeSystem.Instance.GetCurrentTimestamp, "Check out your next sample."));
    }
}
