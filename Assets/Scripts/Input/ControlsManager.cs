using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : SingletonMonobehaviour<ControlsManager>
{
    InputSystemProvider inputSystemProvider;

    protected override void Awake()
    {
        base.Awake();
        inputSystemProvider = new InputSystemProvider();
    }

    public Vector2 MapMoveVector => inputSystemProvider.MapMoveVector;

    void OnDestroy()
    {
        inputSystemProvider.Dispose();
    }
}
