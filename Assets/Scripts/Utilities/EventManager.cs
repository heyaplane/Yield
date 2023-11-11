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
}
