using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonStaySelected : MonoBehaviour
{
    Button button;
    
    Sprite normalSprite;
    Sprite selectedSprite;

    Color normalColor;
    Color selectedColor;

    void Awake()
    {
        button = GetComponent<Button>();
        
        normalSprite = button.image.sprite;
        selectedSprite = button.spriteState.pressedSprite;

        normalColor = button.image.color;
        selectedColor = new Color
        {
            a = normalColor.a,
            r = normalColor.r * 0.8f,
            g = normalColor.g * 0.8f,
            b = normalColor.b * 0.8f
        };
    }

    public void DeselectButton()
    {
        button.image.sprite = normalSprite;
        button.image.color = normalColor;
    }

    public void SelectButton()
    {
        button.image.sprite = selectedSprite;
        button.image.color = selectedColor;
    }
}
