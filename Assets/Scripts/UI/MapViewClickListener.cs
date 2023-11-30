using System;
using UnityEngine;

public class MapViewClickListener : MonoBehaviour
{
    [SerializeField] WaferMapViewManager waferMapViewManager;

    bool isMeasurementToggled; 
    
    [SerializeField] MeasurementLine measurementLinePrefab;
    MeasurementLine currentMeasurement;
    public float CurrentMeasurementValue => currentMeasurement != null ? currentMeasurement.MeasurementValue : -1f;

    bool isDrawingMeasurement;
    Vector2 startPoint;
    Vector2 endPoint;

    public void HandleMeasurementToggle()
    {
        isMeasurementToggled = !isMeasurementToggled;
        if (isMeasurementToggled && currentMeasurement != null)
        {
            Destroy(currentMeasurement.gameObject);
            currentMeasurement = null;
        }
    }
    
    public void OnMouseDown()
    {
        if (!isMeasurementToggled) return;
        
        if (!isDrawingMeasurement)
        {
            if (currentMeasurement != null)
                Destroy(currentMeasurement.gameObject);
            currentMeasurement = Instantiate(measurementLinePrefab);
            currentMeasurement.Initialize(waferMapViewManager.CurrentScaleFactor);
            
            startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endPoint = startPoint;
            currentMeasurement.DrawLine(startPoint, endPoint);
            isDrawingMeasurement = true;
        }
        
        else if (isDrawingMeasurement)
        {
            endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMeasurement.FinishLine(startPoint, endPoint);
            isDrawingMeasurement = false;
            isMeasurementToggled = false;
        }
    }

    void Update()
    {
        if (!isDrawingMeasurement || currentMeasurement == null) return;
        
        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMeasurement.DrawLine(startPoint, endPoint);
    }

    public void DeleteCurrentMeasurement()
    {
        if (currentMeasurement != null)
            Destroy(currentMeasurement.gameObject);
    }
}
