using UnityEngine.InputSystem;

public class PausedGameState : BaseGameState
{
    public PausedGameState(GameStateMachine sm) : base(sm) { }
    public override GameState Name => GameState.Paused;
    
    InputSystemProvider inputSystem;
    
    public override void Update()
    {
    }

    public override void Exit()
    {
        inputSystem.DisableUIToggleControls(sm.PauseToggleAction, EventManager.OnUIToggleRequested);
        
        if (sm.NextState == sm.GetStateFromName(GameState.Loading))
            SaveManager.Instance.SaveGameData();
    }

    public override void OnPauseRequested(InputActionReference toggleAction)
    {
    }

    public override void OnUnpauseRequested()
    {
        sm.ChangeState(sm.PreviousState);
    }

    public override void OnRequestSceneTransition()
    {
        sm.ChangeState(sm.loading);
    }

    public override void UpdateAllowedInput(InputSystemProvider inputSystem)
    {
        this.inputSystem = inputSystem;
        inputSystem.EnableUIToggleControls(sm.PauseToggleAction, EventManager.OnUIToggleRequested);
    }
}