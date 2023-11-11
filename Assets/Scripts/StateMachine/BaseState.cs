using System;
using UnityEngine;

public abstract class BaseState<T>
{
    public abstract T Name { get; }
    
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
