using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : SingletonMonobehaviour<ControlsManager>
{
    PlayerInputActions playerInputActions;

    protected override void Awake()
    {
        base.Awake();
        playerInputActions = new PlayerInputActions();
        
        playerInputActions.Player.Enable();

        playerInputActions.Player.Quit.performed += OnPlayerQuitKeyPressed;
        playerInputActions.Player.Delete.performed += OnPlayerDeleteKeyPressed;
    }

    public Vector2 SampleMoveVector => playerInputActions.Player.MoveSample.ReadValue<Vector2>();

    void OnPlayerQuitKeyPressed(InputAction.CallbackContext context) => EventManager.OnQuitKeyPressed(context);
    void OnPlayerDeleteKeyPressed(InputAction.CallbackContext context) => EventManager.OnDeleteKeyPressed(context);
}
