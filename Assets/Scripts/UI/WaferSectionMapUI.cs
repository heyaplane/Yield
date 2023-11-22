using System;
using System.Collections.Generic;
using UnityEngine;

public class WaferSectionMapUI : MonoBehaviour
{
    [SerializeField] WaferSectionUI sectionImage_8x8;
    [SerializeField] WaferSectionUI sectionImage_16x16;
    [SerializeField] WaferSectionUI sectionImage_32x32;

    [SerializeField] Transform sectionParent;
    [SerializeField] RectTransform waferRectTransform;

    Dictionary<int, WaferSectionUI> sectionImageLookup;

    WaferSectionUI currentlySelectedWaferSection;

    void OnEnable()
    {
        sectionImageLookup = new Dictionary<int, WaferSectionUI>
        {
            {8, sectionImage_8x8},
            {16, sectionImage_16x16},
            {32, sectionImage_32x32}
        };
    }

    public void Initialize(VirtualReport virtualReport, string featureName, Action<WaferSection> OnWaferSectionSelected)
    {
        var waferMap = virtualReport.WaferMap;
        
        if (!sectionImageLookup.TryGetValue(waferMap.SectionDimSize, out var sectionImage))
        {
            Debug.LogError("Could not find the right section image!");
            return;
        }

        var waferLayout = new WaferLayout(waferMap.ChunkDimSize, waferMap.SectionDimSize);
        var waferSections = waferLayout.DivideWaferIntoSections(.4967f);
        float waferDimSize = waferRectTransform.rect.size.x;
        foreach (var section in waferSections)
        {
            var newSection = Instantiate(sectionImage, sectionParent);
            newSection.OnWaferSectionSelected += sectionUI => HandleWaferSectionSelected(sectionUI, section, OnWaferSectionSelected);
            newSection.transform.localPosition = 
                new Vector2( section.NormalizedSectionCoordinate.x * waferDimSize - waferDimSize/2, -section.NormalizedSectionCoordinate.y * waferDimSize + waferDimSize/2);
            newSection.gameObject.SetActive(true);

            if (virtualReport.TryGetReportEntry(section.SectionLocationAsString, featureName, out var reportEntry))
                newSection.ChangeSprite(reportEntry.State);
            else
                newSection.ChangeSprite(ReportEntryState.Default);
        }
    }

    void HandleWaferSectionSelected(WaferSectionUI waferSectionUI, WaferSection section, Action<WaferSection> onWaferSectionSelected)
    {
        if (currentlySelectedWaferSection != null)
            currentlySelectedWaferSection.ResetSectionSelected();

        currentlySelectedWaferSection = waferSectionUI;
        onWaferSectionSelected?.Invoke(section);
    }
}
