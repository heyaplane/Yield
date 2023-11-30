using System;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource; 
    
    void OnEnable()
    {
        EventManager.OnAudioClipTriggeredEvent += PlayClip;
    }

    void OnDisable()
    {
        EventManager.OnAudioClipTriggeredEvent -= PlayClip;
    }

    void PlayClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
