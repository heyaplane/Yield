using UnityEngine;
using UnityEngine.InputSystem;

public class InGameState : BaseGameState
{
    public InGameState(GameStateMachine sm) : base(sm) { }
    public override GameState Name => GameState.InGame;

    InputSystemProvider inputSystem;

    public override void Enter()
    {
        base.Enter();
        if (sm.PreviousState == sm.GetStateFromName(GameState.Loading))
            SaveManager.Instance.LoadGameData();
    }

    public override void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Tab))
        // {
        //     GameManager.Instance.RequestSceneTransition();
        // }
    }

    public override void Exit()
    {
        inputSystem.DisableAllPlayerControls();
        inputSystem.DisableUIToggleControls(null, EventManager.OnUIToggleRequested);
        
        if (sm.NextState == sm.GetStateFromName(GameState.Loading))
            SaveManager.Instance.SaveGameData();
    }

    public override void OnPauseRequested(InputActionReference toggleAction)
    {
        sm.PauseToggleAction = toggleAction;
        sm.ChangeState(sm.GetStateFromName(GameState.Paused));
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
        this.inputSystem = inputSystem;
        inputSystem.EnableAllPlayerControls();
        inputSystem.EnableUIToggleControls(null, EventManager.OnUIToggleRequested);
    }
}