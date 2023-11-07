using System;
using TMPro;
using UnityEngine;

public class MeasurementLine : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] TextMeshProUGUI measurementText;

    bool isMeasuring;
    bool isDeadLine;
    Vector2 startPoint;
    Vector2 endPoint;

    Sprite currentSprite;

    public void Initialize(Sprite currentSprite)
    {
        this.currentSprite = currentSprite;
    }

    void Update()
    {
        if (isDeadLine) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!isMeasuring)
            {
                startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                endPoint = startPoint;
                measurementText.gameObject.SetActive(true);
                UpdateMeasurement();
                isMeasuring = true;
            }

            else
            {
                isMeasuring = false;
                isDeadLine = true;
            }
        }

        else if (isMeasuring)
        {
            endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DrawLine(startPoint, endPoint);
            UpdateMeasurement();
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    void UpdateMeasurement()
    {
        measurementText.rectTransform.pivot = new Vector2(0.5f, 0);
        measurementText.rectTransform.position = (startPoint + endPoint) / 2;

        float distance = Vector2.Distance(startPoint, endPoint) / Vector2.Distance(Vector2.zero, currentSprite.bounds.size);
        measurementText.text = $"{distance:F4}";
    }
}
