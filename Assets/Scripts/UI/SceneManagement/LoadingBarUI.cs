using UnityEngine;
using UnityEngine.UI;

public class LoadingBarUI : MonoBehaviour, IShowProgress
{
    [SerializeField] Image progressBar;
    
    public void UpdateProgress(float progress)
    {
        progressBar.fillAmount = progress;
    }

    public GameObject GetGameObject() => gameObject;
}
