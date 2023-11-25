using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaferSectionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image sectionImage;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite selectedSprite;
    [SerializeField] Sprite dataExistSprite;
    [SerializeField] Sprite passSprite;
    [SerializeField] Sprite failSprite;

    Dictionary<ReportEntryState, Sprite> spriteLookup;
    
    bool sectionSelected;
    public ReportEntryState PersistentState { get; set; } = ReportEntryState.Default;
    public event Action<WaferSectionUI> OnWaferSectionSelected;

    void OnEnable()
    {
        spriteLookup = new Dictionary<ReportEntryState, Sprite>
        {
            {ReportEntryState.Default, defaultSprite},
            {ReportEntryState.Selected, selectedSprite},
            {ReportEntryState.DataExist, dataExistSprite},
            {ReportEntryState.Pass, passSprite},
            {ReportEntryState.Fail, failSprite}
        };
        
        ChangeSprite(PersistentState);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sectionSelected) return;
        
        ChangeSprite(selectedSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (sectionSelected) return;
        
        ChangeSprite(PersistentState);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        sectionSelected = true;
        ChangeSprite(selectedSprite);
        OnWaferSectionSelected?.Invoke(this);
    }

    public void ResetSectionSelected()
    {
        sectionSelected = false;
        ChangeSprite(PersistentState);
    }

    public void ChangeSprite(ReportEntryState sectionUIState) => ChangeSprite(spriteLookup[sectionUIState]);
    void ChangeSprite(Sprite sprite) => sectionImage.sprite = sprite;
}
