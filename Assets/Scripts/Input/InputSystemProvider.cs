
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemProvider
{
    PlayerInputActions playerInputActions;
    
    string PLAYER_INPUT_BINDINGS = PlayerPrefsSaveSystem.inputOverrideKey;
    
    public InputSystemProvider()
    {
        playerInputActions = new PlayerInputActions();
        
        if (PlayerPrefs.HasKey(PLAYER_INPUT_BINDINGS))//Before inputs have been enabled, after construction of playerInputActions
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_INPUT_BINDINGS));
        
        EventManager.OnBindingTextRequestedEvent += GetBindingText;
        EventManager.OnRebindingInputRequestedEvent += RebindInput;
        EventManager.OnGameStateChangedEvent += UpdateInputStatus;
        
        playerInputActions.Player.Enable();

        playerInputActions.Player.Quit.performed += OnPlayerQuitKeyPressed;
        playerInputActions.Player.Delete.performed += OnPlayerDeleteKeyPressed;
    }
    
    public Vector2 MapMoveVector => playerInputActions.Player.MoveSample.ReadValue<Vector2>();

    void OnPlayerQuitKeyPressed(InputAction.CallbackContext context) => EventManager.OnQuitKeyPressed(context);
    void OnPlayerDeleteKeyPressed(InputAction.CallbackContext context) => EventManager.OnDeleteKeyPressed(context);
    
    public void EnableAllPlayerControls() => playerInputActions.Player.Enable();
    public void DisableAllPlayerControls() {
        playerInputActions.Player.Disable();
    }

    public void EnableUIToggleControls(InputActionReference toggleRef, Action<InputAction.CallbackContext> callback)
    {
        playerInputActions.Player.Escape.Enable();
        playerInputActions.Player.Escape.performed += callback;
        
        if (toggleRef == null) return;
        var toggleAction = playerInputActions.FindAction(toggleRef.action.id.ToString());
        toggleAction.Enable();
        toggleAction.performed += callback;
    }

    public void DisableUIToggleControls(InputActionReference toggleRef, Action<InputAction.CallbackContext> callback)
    {
        playerInputActions.Player.Escape.Disable();
        playerInputActions.Player.Escape.performed -= callback;
        
        if (toggleRef == null) return;
        var toggleAction = playerInputActions.FindAction(toggleRef.action.id.ToString());
        toggleAction.Disable();
        toggleAction.performed -= callback;
    }

    void UpdateInputStatus(GameState state) => GameManager.Instance.RequestInputStatusUpdate(this);
    
    string GetBindingText(InputActionReference actionRef, int bindingIndex) 
    {
        var action = playerInputActions.FindAction(actionRef.name);
        return action.bindings[bindingIndex].ToDisplayString();
    }
    
    void RebindInput(InputActionReference actionRef, int bindingIndex, Action reboundAction) 
    {
        DisableAllPlayerControls();
        var inputAction = playerInputActions.FindAction(actionRef.name);
        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback => {
            callback.Dispose();
            EnableAllPlayerControls();
            reboundAction();
            string str = playerInputActions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(PLAYER_INPUT_BINDINGS, str);
            PlayerPrefs.Save();
        }).Start();
    }

    public void Dispose()
    {
        playerInputActions.Player.Quit.performed -= OnPlayerQuitKeyPressed;
        playerInputActions.Player.Delete.performed -= OnPlayerDeleteKeyPressed;
        
        EventManager.OnGameStateChangedEvent -= UpdateInputStatus;
        EventManager.OnBindingTextRequestedEvent -= GetBindingText;
        EventManager.OnRebindingInputRequestedEvent -= RebindInput;
    }
}
