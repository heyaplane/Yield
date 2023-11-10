using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    [SerializeField] InputActionReference escapeAction;

    Stack<BaseUI> currentlyOpenUI;

    RectTransform canvasRectTransform;
    RectTransform currentTooltip;

    protected override void Awake()
    {
        base.Awake();

        currentlyOpenUI = new Stack<BaseUI>();
    }

    void OnEnable()
    {
        EventManager.OnUIToggleRequestedEvent += CloseUIAtStackTop;
        //EventManager.CallBeforeSceneUnloadEvent += ClearUIStack;
    }

    void OnDisable()
    {
        EventManager.OnUIToggleRequestedEvent -= CloseUIAtStackTop;
        //EventManager.CallBeforeSceneUnloadEvent -= ClearUIStack;
    }

    void Update()
    {
        if (currentTooltip == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, null, out var localPos);
        currentTooltip.localPosition = localPos;
    }

    public void SetUpUI(BaseUI baseUI, bool isPermanent)
    {
        baseUI.UIWindow.SetActive(true);
        
        if (!isPermanent)
            UpdateUIStack(baseUI);
    }

    public void TearDownUI(BaseUI baseUI, bool isPermanent)
    {
        baseUI.UIWindow.SetActive(false);
        
        if (!isPermanent)
            UpdateUIStack(baseUI);
    }

    void UpdateUIStack(BaseUI ui)
    {
        if (ui.IsActive)
        {
            currentlyOpenUI.Push(ui);
            GameManager.Instance.RequestPauseState(ui.ToggleAction);
        }
        
        else if (currentlyOpenUI.Count > 0)
        {
            var poppedUI = currentlyOpenUI.Pop();
            if (poppedUI != ui) Debug.LogError("Incorrect UI was popped!");
        }
        
        else
            Debug.LogError("Tried to pop empty stack.");
    }

    void CloseUIAtStackTop(InputAction.CallbackContext context)
    {
        bool uiOpen = currentlyOpenUI.TryPeek(out var ui);
        bool uiButtonPressed = context.action == null;
        bool escKeyPressed = false;
        bool toggleKeyPressed = false;

        if (!uiButtonPressed)
        {
            escKeyPressed = context.action.id == escapeAction.action.id;
            if (ui != null && ui.ToggleAction != null) toggleKeyPressed = context.action.id == ui.ToggleAction.action.id;
        }

        if (uiOpen && (uiButtonPressed || escKeyPressed || toggleKeyPressed))
        {
            ui.CloseWindow();
            if (currentlyOpenUI.Count == 0)
                GameManager.Instance.RequestUnpause();
        }

        else if (escKeyPressed)
        {
            GameManager.Instance.RequestGamePause();
        }
    }

    void ClearUIStack() => currentlyOpenUI.Clear();
}
