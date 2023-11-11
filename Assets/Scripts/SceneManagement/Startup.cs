using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    void Awake()
    {
        SceneManager.LoadScene(Constants.PerstSceneName, LoadSceneMode.Additive);
    }
}
