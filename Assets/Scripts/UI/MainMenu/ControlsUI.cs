using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsUI : BaseUI
{
    [SerializeField] Button closeButton;
    [SerializeField] Button moveLeftButton; //Turn this into an array, and then store as a dictionary?
    [SerializeField] TextMeshProUGUI rebindInstructionsText;

    TextMeshProUGUI moveLeftButtonText;
    BindingButtonUI moveLeftBindingButton;

    void Awake()
    {
        moveLeftButtonText = moveLeftButton.GetComponentInChildren<TextMeshProUGUI>();
        moveLeftBindingButton = moveLeftButton.GetComponent<BindingButtonUI>();
    }

    void OnEnable()
    {
        closeButton.onClick.AddListener(HandleCloseButton);
        moveLeftButton.onClick.AddListener(() => RebindInputFromButtonPress(moveLeftBindingButton, moveLeftButtonText));
        
        UpdateBindingButtonText(moveLeftBindingButton, moveLeftButtonText);
    }

    void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        moveLeftButton.onClick.RemoveAllListeners();
    }

    void HandleCloseButton()
    {
        SaveManager.Instance.SavePlayerPrefs();
        CloseWindow();
        OnCancelAction?.Invoke();
    }
    
    void RebindInputFromButtonPress(BindingButtonUI bindingButton, TextMeshProUGUI buttonText)
    {
        if (bindingButton == null) return;
        
        buttonText.text = "???";
        rebindInstructionsText.gameObject.SetActive(true);
        
        EventManager.OnRebindingInputRequested(
            bindingButton.Action, 
            bindingButton.BindingIndex, 
            () => {
                UpdateBindingButtonText(bindingButton, buttonText);
                rebindInstructionsText.gameObject.SetActive(false);
            });
    }

    void UpdateBindingButtonText(BindingButtonUI bindingButton, TextMeshProUGUI buttonText) 
    {
        if (bindingButton == null) return;

        buttonText.text = EventManager.OnBindingTextRequested(bindingButton.Action, bindingButton.BindingIndex);
    }
}
