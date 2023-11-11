using UnityEngine.InputSystem;

public class MainMenuGameState : BaseGameState
{
    public override GameState Name => GameState.MainMenu;

    public MainMenuGameState(GameStateMachine sm) : base(sm) { }

    public override void Enter()
    {
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
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
