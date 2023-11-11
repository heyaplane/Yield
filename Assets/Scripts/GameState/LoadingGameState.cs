using UnityEngine.InputSystem;

public class LoadingGameState : BaseGameState
{
    public override GameState Name => GameState.Loading;

    public LoadingGameState(GameStateMachine sm) : base(sm) { }
    
    public override void Enter()
    {
        base.Enter();
        EventManager.CallAfterSceneLoadEvent += OnSceneLoadExit;
        SceneController.Instance.SwitchToNewScene();
    }

    public override void Update() { }
    public override void Exit() { }
    public override void OnPauseRequested(InputActionReference toggleAction) { }
    public override void OnUnpauseRequested() { }
    public override void OnRequestSceneTransition() { }
    public override void UpdateAllowedInput(InputSystemProvider inputSystem) { }

    void OnSceneLoadExit(SceneSO scene)
    {
        EventManager.CallAfterSceneLoadEvent -= OnSceneLoadExit;
        
        sm.ChangeState(sm.GetStateFromName(scene.GameStateOnLoading));
    }
}
