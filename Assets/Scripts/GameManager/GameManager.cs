﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    void OnEnable()
    {
        EventManager.OnQuitKeyPressedEvent += QuitGame;
    }

    public void QuitGame(InputAction.CallbackContext context)
    {
        Application.Quit();
    }
}
