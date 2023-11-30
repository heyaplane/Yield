using System;
using UnityEngine.InputSystem;

public class EventManager
{
    #region Controls

    public static event Action<InputAction.CallbackContext> OnQuitKeyPressedEvent;

    public static void OnQuitKeyPressed(InputAction.CallbackContext context)
    {
        OnQuitKeyPressedEvent?.Invoke(context);
    }

    public static event Action<InputAction.CallbackContext> OnDeleteKeyPressedEvent;
    public static void OnDeleteKeyPressed(InputAction.CallbackContext context)
    {
        OnDeleteKeyPressedEvent?.Invoke(context);
    }
    
    public static event Action<InputAction.CallbackContext> OnUIToggleRequestedEvent;
    public static void OnUIToggleRequested(InputAction.CallbackContext context)
    {
        OnUIToggleRequestedEvent?.Invoke(context);
    }
    
    public static event Func<InputActionReference, int, string> OnBindingTextRequestedEvent;
    public static string OnBindingTextRequested(InputActionReference actionRef, int bindingIndex) {
        string str = OnBindingTextRequestedEvent?.Invoke(actionRef, bindingIndex);
        return str;
    }
    
    public static event Action<InputActionReference, int, Action> OnRebindingInputRequestedEvent;
    public static void OnRebindingInputRequested(InputActionReference actionRef, int bindingIndex, Action reboundAction) {
        OnRebindingInputRequestedEvent?.Invoke(actionRef, bindingIndex, reboundAction);
    }

    #endregion

    #region Game State

    public static event Action<GameState> OnGameStateChangedEvent;
    public static void OnGameStateChanged(GameState state)
    {
        OnGameStateChangedEvent?.Invoke(state);
    }

    public static event Action<GameOverState, int> OnGameOverEvent;
    public static void OnGameOver(GameOverState gameOverState, int deviceYield)
    {
        OnGameOverEvent?.Invoke(gameOverState, deviceYield);
    }

    #endregion

    #region UI Management

    public static event Action OnPauseMenuTriggeredEvent;
    public static void OnPauseMenuTriggered()
    {
        OnPauseMenuTriggeredEvent?.Invoke();
    }

    #endregion
    
    #region Scene Management

    public static event Action CallBeforeSceneFadeOutEvent;
    public static void CallBeforeSceneFadeOut()
    {
        CallBeforeSceneFadeOutEvent?.Invoke();
    }

    public static event Action CallBeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnload()
    {
        CallBeforeSceneUnloadEvent?.Invoke();
    }
    
    public static event Action CallInBetweenScenesEvent;
    public static void CallInBetweenScenes()
    {
        CallInBetweenScenesEvent?.Invoke();
    }

    public static event Action<SceneSO> CallAfterSceneLoadEvent;
    public static void CallAfterSceneLoad(SceneSO scene)
    {
        CallAfterSceneLoadEvent?.Invoke(scene);
    }

    public static event Action CallAfterSceneFadeInEvent;
    public static void CallAfterSceneFadeIn()
    {
        CallAfterSceneFadeInEvent?.Invoke();
    }

    public static event Action OnRequestMainMenuTransitionEvent;
    public static void OnRequestMainMenuTransition()
    {
        OnRequestMainMenuTransitionEvent?.Invoke();   
    }

    #endregion

    #region TimeManagement

    public static event Action<int> OnMinutesIncrementedEvent;
    public static void OnMinutesIncremented(int currentMinute)
    {
        OnMinutesIncrementedEvent?.Invoke(currentMinute);   
    }
    
    public static event Action<int> OnHoursIncrementedEvent;
    public static void OnHoursIncremented(int currentMinute)
    {
        OnHoursIncrementedEvent?.Invoke(currentMinute);   
    }

    public static event Action<int> OnDaysIncrementedEvent;
    public static void OnDaysIncremented(int currentMinute)
    {
        OnDaysIncrementedEvent?.Invoke(currentMinute);   
    }

    public static event Action OnTimeOutEvent;
    public static void OnTimeOut()
    {
        OnTimeOutEvent?.Invoke();
    }
    
    #endregion

    #region MessageSystem

    public static event Action<ThreadData> OnNewThreadAddedEvent;
    public static void OnNewThreadAdded(ThreadData threadData) => 
        OnNewThreadAddedEvent?.Invoke(threadData);

    public static event Action<MessageData> OnReplyButtonClickedEvent;
    public static void OnReplyButtonClicked(MessageData respondentMessage) =>
        OnReplyButtonClickedEvent?.Invoke(respondentMessage);

    public static event Action<VirtualReport, MessageData> OnReportChosenEvent;
    public static void OnReportChosen(VirtualReport chosenReport, MessageData respondentMessage) => 
        OnReportChosenEvent?.Invoke(chosenReport, respondentMessage);

    #endregion
}
