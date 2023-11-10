using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseUI : MonoBehaviour
{
    [SerializeField] protected GameObject uiWindow;
    public GameObject UIWindow => uiWindow;
    
    [SerializeField] bool isPermanent;
    public bool IsPermanent => isPermanent;

    [SerializeField] InputActionReference toggleAction;
    public InputActionReference ToggleAction => toggleAction;

    public Action OnCancelAction { get; set; }
    public Action<string> OnCancelActionWithMessage { get; set; }

    public bool IsActive => uiWindow.activeSelf;
    
    public virtual void EnableWindow()
    {
        if (UIManager.Instance == null) return;
        UIManager.Instance.SetUpUI(this, isPermanent);
    }

    public virtual void CloseWindow()
    {
        if (UIManager.Instance == null) return;
        UIManager.Instance.TearDownUI(this, isPermanent);
    }
}
