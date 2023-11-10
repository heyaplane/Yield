using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image backgroundImage;
    [SerializeField] TextMeshProUGUI text;
    public TextMeshProUGUI Text => text;

    public event Action<HighlightOnClick> OnClickedAction;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickedAction?.Invoke(this);
    }

    public void MarkAsSelected(Color backgroundSelectedColor) => backgroundImage.color = backgroundSelectedColor;

    public void Deselect()
    {
        var color = backgroundImage.color;
        color.a = 0f;
        backgroundImage.color = color;
    } 
}
