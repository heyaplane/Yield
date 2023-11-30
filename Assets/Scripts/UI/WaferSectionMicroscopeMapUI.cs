using System;
using System.Collections.Generic;
using UnityEngine;

public class WaferSectionMicroscopeMapUI : MonoBehaviour
{
    [SerializeField] GameObject sectionImage_8x8;
    [SerializeField] GameObject sectionImage_16x16;
    [SerializeField] GameObject sectionImage_32x32;

    [SerializeField] Transform sectionParent;
    [SerializeField] RectTransform waferRectTransform;

    Dictionary<int, GameObject> sectionImageLookup;

    [SerializeField] Transform markerImage;

    void OnEnable()
    {
        sectionImageLookup = new Dictionary<int, GameObject>
        {
            {8, sectionImage_8x8},
            {16, sectionImage_16x16},
            {32, sectionImage_32x32}
        };
    }

    public void Initialize()
    {
        foreach (Transform child in sectionParent)
        {
            Destroy(child.gameObject);
        }
        
        if (!sectionImageLookup.TryGetValue(8, out var sectionImage))
        {
            Debug.LogError("Could not find the right section image!");
            return;
        }

        var waferLayout = new WaferLayout(256, 8);
        var waferSections = waferLayout.DivideWaferIntoSections(.4967f);
        float waferDimSize = waferRectTransform.rect.size.x;
        foreach (var section in waferSections)
        {
            var newSection = Instantiate(sectionImage, sectionParent);
            newSection.transform.localPosition = 
                new Vector2( section.NormalizedSectionCoordinate.x * waferDimSize - waferDimSize/2, -section.NormalizedSectionCoordinate.y * waferDimSize + waferDimSize/2);
            newSection.gameObject.SetActive(true);
        }
    }

    public void UpdateMarkerImage(Vector2 normalizedCoordinate)
    {
        
        float waferDimSize = waferRectTransform.rect.size.x;
        markerImage.transform.localPosition = 
            new Vector2( normalizedCoordinate.x * waferDimSize - waferDimSize/2, -normalizedCoordinate.y * waferDimSize + waferDimSize/2);
    }
}
