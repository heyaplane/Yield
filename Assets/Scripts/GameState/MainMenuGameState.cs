using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuGameState : BaseGameState
{
    public override GameState Name => GameState.MainMenu;

    public MainMenuGameState(GameStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        TimeSystem.Instance.StopGameTime();
        Debug.Log("Game time stopped.");
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        TimeSystem.Instance.StartGameTime();
        Debug.Log("Game time started.");
    }

    public override void OnPauseRequested(InputActionReference toggleAction)
    {
    }

    public override void OnUnpauseRequested()
    {
    }

    public override void OnRequestSceneTransition()
    {
        sm.ChangeState(sm.loading);
    }

    public override void UpdateAllowedInput(InputSystemProvider inputSystem)
    {
    }

}
