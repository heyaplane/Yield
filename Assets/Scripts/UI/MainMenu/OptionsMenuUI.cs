using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuUI : BaseUI
{
    [SerializeField] Button changeProfileButton;
    [SerializeField] Button controlsButton;
    [SerializeField] Button cancelButton;

    [SerializeField] BaseUI profileChangeUI;
    [SerializeField] BaseUI controlsUI;
    
    
    void OnEnable()
    {
        changeProfileButton.onClick.AddListener(HandleChangeProfileButton);
        controlsButton.onClick.AddListener(HandleControlsButton);
        cancelButton.onClick.AddListener(HandleCancelButton);

        profileChangeUI.OnCancelAction = EnableWindow;
        controlsUI.OnCancelAction = EnableWindow;
    }
    
    void OnDisable()
    {
        changeProfileButton.onClick.RemoveListener(HandleChangeProfileButton);
        controlsButton.onClick.RemoveListener(HandleControlsButton);
        cancelButton.onClick.RemoveListener(HandleCancelButton);
    }

    void HandleChangeProfileButton()
    {
        CloseWindow();
        profileChangeUI.EnableWindow();
    }

    void HandleControlsButton()
    {
        CloseWindow();
        controlsUI.EnableWindow();
    }

    void HandleCancelButton()
    {
        CloseWindow();
        OnCancelAction?.Invoke();
    }

}
