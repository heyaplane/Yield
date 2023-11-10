using System;
using UnityEngine.InputSystem;

public class EventManager
{
    #region Controls

    public static event Action<InputAction.CallbackContext> OnQuitKeyPressedEvent;
    public static void OnQuitKeyPressed(InputAction.CallbackContext context)
    {
        OnQuitKeyPressedEvent?.Invoke(context);
    }

    public static event Action<InputAction.CallbackContext> OnDeleteKeyPressedEvent;
    public static void OnDeleteKeyPressed(InputAction.CallbackContext context)
    {
        OnDeleteKeyPressedEvent?.Invoke(context);
    }

    #endregion
}
