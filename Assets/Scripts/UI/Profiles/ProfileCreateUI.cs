using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileCreateUI : BaseUI
{
    [SerializeField] TMP_InputField profileNameInput;
    [SerializeField] Button createButton;

    void OnEnable()
    {
        createButton.onClick.AddListener(() => SaveManager.Instance.ChangeProfile(profileNameInput.text));
    }
    
    void OnDisable()
    {
        createButton.onClick.RemoveAllListeners();
    }
}
