using UnityEngine;
using UnityEngine.InputSystem;

public class BindingButtonUI : MonoBehaviour {

    [SerializeField] InputActionReference action;
    public InputActionReference Action => action;

    [SerializeField] int bindingIndex;
    public int BindingIndex => bindingIndex;
}
