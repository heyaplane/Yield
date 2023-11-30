using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(1)] //After SceneController
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [SerializeField] PauseMenuUI pauseMenuUI;
    
    GameStateMachine gameStateMachine;

    void RequestMainMenuTransition() => RequestSceneTransition(SceneController.Instance.MainMenu);
    
    void OnEnable()
    {
        SetStartingSceneAndSaveState();

        EventManager.OnRequestMainMenuTransitionEvent += RequestMainMenuTransition;
    }

    void OnDisable()
    {
        EventManager.OnRequestMainMenuTransitionEvent -= RequestMainMenuTransition;
    }

    IEnumerator Start()
    {
        yield return null;
        
        // This initiates the Loading state & a scene switch
        gameStateMachine = new GameStateMachine();
    }

    void Update() => gameStateMachine?.UpdateCurrentState();



    void SetStartingSceneAndSaveState()
    {
        if (!SaveManager.Instance.TrySetLatestPlayerProfile())
        {
            SceneController.Instance.SetProfileSelectStartingScene();
        }
        
        else
        {
            SceneController.Instance.SetStartingScene();
        }
    }

    public void RequestImmediateLoadTransition() => gameStateMachine.ChangeState(gameStateMachine.GetStateFromName(GameState.Loading));
    public void RequestSceneTransition(SceneSO scene = null)
    {
        SceneController.Instance.SetNextScene(scene);
        gameStateMachine.ActOnState(state => state.OnRequestSceneTransition());
    }

    public void RequestGamePause()
    {
        print("Entered Paused state");
        EventManager.OnPauseMenuTriggered();
        RequestPauseState(null);
    }
    
    public void RequestPauseState(InputActionReference toggleAction) => gameStateMachine.ActOnState(state => state.OnPauseRequested(toggleAction));
    public void RequestUnpause() => gameStateMachine.ActOnState(state => state.OnUnpauseRequested());
    public void RequestInputStatusUpdate(InputSystemProvider inputSystemProvider) => gameStateMachine?.ActOnState(state => state.UpdateAllowedInput(inputSystemProvider));
}

public enum GameOverState
{
    Win, Failed, TimeOut
}
