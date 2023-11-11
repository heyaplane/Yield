using System.Collections;
using UnityEngine;

public interface ITransitionUI
{
    public void SetUpTransition(float finalValue);
    public IEnumerator Transition();
    public void TearDownTransition();

    public GameObject GetGameObject();
}
