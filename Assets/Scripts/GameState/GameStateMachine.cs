using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class GameStateMachine : BaseStateMachine<BaseGameState, GameState>
{
    internal LoadingGameState loading;
    internal MainMenuGameState mainMenu;
    internal InGameState inGame;
    internal PausedGameState paused;

    public InputActionReference PauseToggleAction { get; set; }

    public GameStateMachine()
    {
        InitializeStates();
        ChangeState(GetInitialState());
    }

    public override void InitializeStates()
    {
        loading = new LoadingGameState(this);
        mainMenu = new MainMenuGameState(this);
        inGame = new InGameState(this);
        paused = new PausedGameState(this);
    }
    
    public override BaseState<GameState> GetInitialState() => loading;

    public override BaseGameState GetStateFromName(GameState stateName)
    {
        return stateName switch
        {
            GameState.Loading => loading,
            GameState.MainMenu => mainMenu,
            GameState.InGame => inGame,
            GameState.Paused => paused,
            _ => throw new ArgumentOutOfRangeException(nameof(stateName), stateName, null)
        };
    }
}