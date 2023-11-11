using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveDontSaveUI : BaseUI
{
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;

    void OnEnable()
    {
        confirmButton.onClick.AddListener(HandleConfirmButton);
        cancelButton.onClick.AddListener(HandleCancelButton);
    }

    void OnDisable()
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }

    void HandleConfirmButton()
    {
        CloseWindow();
        SaveManager.Instance.ShouldSaveOnSceneChange = true;
        GameManager.Instance.RequestSceneTransition(SceneController.Instance.MainMenu);
    }
    
    void HandleCancelButton()
    {
        CloseWindow();
        SaveManager.Instance.ShouldSaveOnSceneChange = false;
        GameManager.Instance.RequestSceneTransition(SceneController.Instance.MainMenu);
    }
}
