using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    public void TriggerSound() => EventManager.OnAudioClipTriggered(clip);
}
