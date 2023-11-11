using System;
using UnityEngine;

public abstract class BaseStateMachine<T,U> where T : BaseState<U>
{
    public BaseState<U> PreviousState { get; private set; }
    public BaseState<U> CurrentState { get; private set; }
    public BaseState<U> NextState { get; private set; }
    
    public abstract BaseState<U> GetInitialState();
    public abstract void InitializeStates();
    
    public abstract T GetStateFromName(U stateName);

    public void ChangeState(BaseState<U> nextState)
    {
        NextState = nextState;
        CurrentState?.Exit();

        PreviousState = CurrentState;
        CurrentState = nextState;
        CurrentState.Enter();
        NextState = null;
    }

    public void UpdateCurrentState() => CurrentState?.Update();
    public void ActOnState(Action<T> action) => action((T) CurrentState);
    public V FuncOnState<V>(Func<T, V> function) => function((T) CurrentState);
}
