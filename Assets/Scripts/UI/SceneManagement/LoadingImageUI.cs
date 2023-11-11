using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingImageUI : MonoBehaviour, ISerializationCallbackReceiver
{
    [NonSerialized] IShowProgress progress;
    [NonSerialized] ITransitionUI transitionUI;

    [SerializeField] GameObject Progress;
    [SerializeField] GameObject TransitionUI;
    
    public bool IsTransitioning { get; private set; }

    void Awake()
    {
        progress = Progress.GetComponent<IShowProgress>();
        transitionUI = TransitionUI.GetComponent<ITransitionUI>();
    }

    public void SetUp()
    {
        gameObject.SetActive(true);    
    }

    public void TearDown()
    {
        Destroy(gameObject);
    }

    public IEnumerator Transition(int finalAlpha)
    {
        IsTransitioning = true;
        
        transitionUI.SetUpTransition(finalAlpha);
        yield return transitionUI.Transition();
        transitionUI.TearDownTransition();
        
        IsTransitioning = false;
    }

    public IEnumerator UpdateProgressUI(AsyncOperation asyncOperation, float offset)
    {
        Progress.SetActive(true);
        
        while (!asyncOperation.isDone)
        {
            progress.UpdateProgress(asyncOperation.progress / 2 + offset);
            yield return null;
        }
        
        Progress.SetActive(false);
    }

    public void OnBeforeSerialize()
    {
        if (progress != null) Progress = progress.GetGameObject();
        if (transitionUI != null) TransitionUI = transitionUI.GetGameObject();
    }

    public void OnAfterDeserialize() { }
}
