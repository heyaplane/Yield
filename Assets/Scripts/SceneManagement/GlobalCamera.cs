using System;
using UnityEngine;

public class GlobalCamera : MonoBehaviour
{
    void Awake()
    {
        EventManager.CallBeforeSceneFadeOutEvent += () => gameObject.SetActive(true);
        EventManager.CallAfterSceneFadeInEvent += () => gameObject.SetActive(false);
    }
}
