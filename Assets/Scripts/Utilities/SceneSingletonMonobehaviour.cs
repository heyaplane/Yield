using System;
using UnityEngine;

public abstract class SceneSingletonMonobehaviour<T> : SingletonMonobehaviour<T> where T : MonoBehaviour
{
    protected void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
