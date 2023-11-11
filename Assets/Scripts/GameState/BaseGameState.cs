using UnityEngine.InputSystem;

public abstract class BaseGameState : BaseState<GameState>
{
    protected GameStateMachine sm;
    public BaseGameState(GameStateMachine sm)
    {
        this.sm = sm;
    }
    
    public override void Enter()
    {
        EventManager.OnGameStateChanged(Name);
    }

    public override void Update()
    {
        
    }

    public abstract void OnPauseRequested(InputActionReference toggleAction);
    public abstract void OnUnpauseRequested();
    public abstract void OnRequestSceneTransition();
    public abstract void UpdateAllowedInput(InputSystemProvider inputSystem);
}

public enum GameState
{
    Loading,
    MainMenu,
    InGame,
    Paused
}

