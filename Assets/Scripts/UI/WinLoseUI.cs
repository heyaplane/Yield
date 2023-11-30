using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseUI : BaseUI
{
    [SerializeField] TextMeshProUGUI winLoseText;
    [SerializeField] TextMeshProUGUI yieldText;
    [SerializeField] Button mainMenuButton;

    bool isGameOver;

    void OnEnable()
    {
        mainMenuButton.onClick.AddListener(HandleMainMenuButton);

        EventManager.OnGameOverEvent += HandleGameOver;
    }

    void OnDisable()
    {
        mainMenuButton.onClick.RemoveAllListeners();

        EventManager.OnGameOverEvent -= HandleGameOver;
    }

    void HandleGameOver(GameOverState gameOverState, int deviceYield)
    {
        if (isGameOver) return;
        EnableWindow();
        
        switch (gameOverState)
        {
            case GameOverState.Win:
                PlayerWonMessage(deviceYield);
                break;
            case GameOverState.Failed:
                PlayerLostMessage(deviceYield);
                break;
            case GameOverState.TimeOut:
                TimesUpMessage(deviceYield);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameOverState), gameOverState, null);
        }

        isGameOver = true;
    }

    public void PlayerWonMessage(int deviceYield)
    {
        winLoseText.text = "YOU WON!";
        yieldText.text = $"Device Yield: {deviceYield}%";
    }

    public void PlayerLostMessage(int deviceYield)
    {
        winLoseText.text = "YOU FAILED!";
        yieldText.text = $"Device Yield: {deviceYield}%";
    }

    public void TimesUpMessage(int deviceYield)
    {
        winLoseText.text = "TIME'S UP!";
        yieldText.text = $"Device Yield: {deviceYield}%";
    }

    void HandleMainMenuButton()
    {
        CloseWindow();
        SaveManager.Instance.ShouldSaveOnSceneChange = false;
        GameManager.Instance.RequestSceneTransition(SceneController.Instance.MainMenu);
    }
}
