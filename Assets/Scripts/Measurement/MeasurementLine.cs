using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeasurementLine : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] TextMeshProUGUI measurementText;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] Vector3 colliderOffset;

    bool isSelected;
    bool isDeadLine;
    float scaleFactor;

    public void Initialize(float scaleFactor)
    {
        this.scaleFactor = scaleFactor;
        EventManager.OnDeleteKeyPressedEvent += HandleDeleteKeyPressed;
    }

    public void DrawLine(Vector2 start, Vector2 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        
        UpdateMeasurement(start, end);
    }

    public void FinishLine(Vector2 start, Vector2 end)
    {
        DrawLine(start, end);
        FitColliderToLine(start, end);
    }
    
    void UpdateMeasurement(Vector2 start, Vector2 end)
    {
        measurementText.rectTransform.pivot = new Vector2(0.5f, 0);
        measurementText.rectTransform.position = (start + end) / 2;

        float distance = Vector2.Distance(start, end) * scaleFactor;
        measurementText.text = $"{distance:F4}";
    }

    void OnMouseDown()
    {
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        measurementText.color = Color.red;
        isSelected = true;
    }

    void HandleDeleteKeyPressed(InputAction.CallbackContext context)
    {
        if (!isSelected) return;
        EventManager.OnDeleteKeyPressedEvent -= HandleDeleteKeyPressed;
        Destroy(gameObject);
    }

    void FitColliderToLine(Vector2 start, Vector2 end)
    {
        Bounds bounds = new Bounds((start + end) / 2, Vector3.zero);
        var positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        Array.ForEach(positions, x => bounds.Encapsulate(x));

        boxCollider.offset = bounds.center;
        boxCollider.size = bounds.size + colliderOffset;
    }
}
