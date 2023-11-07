using System;
using UnityEngine;

public class ControlsManager : SingletonMonobehaviour<ControlsManager>
{
    PlayerInputActions playerInputActions;

    protected override void Awake()
    {
        base.Awake();
        playerInputActions = new PlayerInputActions();
        
        playerInputActions.Player.Enable();
    }

    public Vector2 SampleMoveVector => playerInputActions.Player.MoveSample.ReadValue<Vector2>();
}
