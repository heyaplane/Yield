using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnterGameNameUI : BaseUI
{
    [SerializeField] Button createButton;
    [SerializeField] TMP_InputField nameInput;

    void OnEnable()
    {
        createButton.onClick.AddListener(HandleCreateButton);
    }

    void OnDisable()
    {
        createButton.onClick.RemoveAllListeners();
    }

    void HandleCreateButton()
    {
        CloseWindow();
        OnCancelActionWithMessage?.Invoke(nameInput.text);
    }
}
