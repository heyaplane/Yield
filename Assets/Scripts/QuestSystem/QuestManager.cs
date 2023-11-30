using System;
using UnityEngine;

public class QuestManager : SingletonMonobehaviour<QuestManager>
{
    ThreadData thread;
    MessageSender outsideSender;

    [SerializeField] ErrorEvolver errorEvolver;
    [SerializeField] QuestSO startingQuest;

    bool hasSentWelcomeMessage;

    void OnEnable()
    {
        outsideSender = new MessageSender("Outside", "Circle");
        
        EventManager.OnReportChosenEvent += HandleReportSubmitted;
        EventManager.OnDaysIncrementedEvent += HandleDaysIncremented;
    }

    void OnDisable()
    {
        EventManager.OnReportChosenEvent -= HandleReportSubmitted;
        EventManager.OnDaysIncrementedEvent -= HandleDaysIncremented;
    }

    void Start()
    {
        WaferManager.Instance.ActiveWafer = startingQuest.StartingWafer;
        
        SendWelcomeMessage();
    }

    public void SendWelcomeMessage()
    {
        thread = MessageSystemManager.Instance.AddThread("Need your help!");
        string messageString = "Hey thanks for agreeing to help us out at the last minute like this.\n"
                               + "Our metrology guy got a bad case of hay fever yesterday and couldn't make it in.\n"
                               + "We've got a PDK lock due in 24 hours and need to finalize the litho process for our new MEMS device.\n"
                               + "The details aren't terribly important. Here's what we really need you to do:\n"
                               + "1. Locate the device patterns on the first wafer we've provided for you. Should already be loaded in the microscope.\n"
                               + "2. Measure the width of the radial comb features on the device pattern. These are the lines that look like they are interlaced. Should be about 2 µm.\n"
                               + "3. The microscope resolution isn't great, so you may need to make multiple measurements within a device to get a representative sample.\n"
                               + "4. There are also multiple devices patterned in a single sector, it would be a good idea to characterize more than one per sector.\n"
                               + "5. Characterize as many sectors as you need to give us a process recommendation in the end. The recommendation can be one of the following:\n"
                               + "     a. All in-spec\n"
                               + "     b. All out-of-spec\n"
                               + "     c. Out-of-spec on the edge of the wafer only\n"
                               + "     d. Out-of-spec in the middle of the wafer only\n"
                               + "6. Create a report and reply to this message by clicking on it and selecting \"Reply\" at the top. We'll generate a new wafer based on your recommendation, or lock the process if you think things looks good.\n" 
                               + "\nWe're relying on your recommendation to help us tune the process. Again, we only have 24 hours before process lock. Good luck!";
        thread.AddMessage(new MessageData(outsideSender, TimeSystem.Instance.GetCurrentTimestamp, messageString));

        hasSentWelcomeMessage = true;
    }
    
    public void SendReportMessage(MessageSender sender, VirtualReport reportFile)
    {
        thread.AddMessage(new MessageData(sender, TimeSystem.Instance.GetCurrentTimestamp, $"Here's my report for the measured samples:\n{reportFile.FileName}"));
    }
    
    void HandleReportSubmitted(VirtualReport reportFile, MessageData messageData)
    {
        if (reportFile == null || messageData == null) return;
        if (reportFile.ProcessRecommendation == ErrorType.Passing)
        {
            if (WaferManager.Instance.ActiveWafer == startingQuest.PassingWafer)
            {
                EventManager.OnGameOver(GameOverState.Win, WaferManager.Instance.GetDeviceYield());
                return;
            }
            EventManager.OnGameOver(GameOverState.Failed, WaferManager.Instance.GetDeviceYield());
            return;
        }
        
        SendReportMessage(new MessageSender("Player", "Square"), reportFile);

        var nextWaferEvolution = errorEvolver.GetNextWaferEvolution(startingQuest, reportFile.WaferData, reportFile.ProcessRecommendation);
        WaferManager.Instance.ActiveWafer = nextWaferEvolution;
        thread.AddMessage(new MessageData(outsideSender, TimeSystem.Instance.GetCurrentTimestamp, "Check out your next sample."));
    }

    public object CaptureSaveData() => hasSentWelcomeMessage;

    public void RestoreSaveData(object saveData)
    {
        if ((bool) saveData)
        {
            MessageSystemManager.Instance.DeleteThread(thread);
            thread = MessageSystemManager.Instance.Threads[0];
        }
    }

    void HandleDaysIncremented(int i)
    {
        EventManager.OnGameOver(GameOverState.TimeOut, WaferManager.Instance.GetDeviceYield());
    }
}
