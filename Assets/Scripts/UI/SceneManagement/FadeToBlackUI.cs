using System.Collections;
using UnityEngine;

public class FadeToBlackUI : MonoBehaviour, ITransitionUI
{
    [SerializeField] CanvasGroup faderCanvasGroup;
    [SerializeField] float fadeDuration;

    float finalAlpha;
    float fadeSpeed;
    
    public void SetUpTransition(float finalValue)
    {
        faderCanvasGroup.blocksRaycasts = true;
        finalAlpha = finalValue;

        fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;
    }

    public IEnumerator Transition()
    {
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void TearDownTransition()
    {
        faderCanvasGroup.blocksRaycasts = false;
    }

    public GameObject GetGameObject() => gameObject;
}
