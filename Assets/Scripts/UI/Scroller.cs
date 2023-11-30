using System;
using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] float xSpeed, ySpeed;

    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.position + new Vector2(xSpeed, ySpeed) * Time.deltaTime, rawImage.uvRect.size);
    }
}
