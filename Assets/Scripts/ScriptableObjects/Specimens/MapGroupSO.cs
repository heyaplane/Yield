using UnityEngine;

[CreateAssetMenu(fileName = "LMSpecimenSO", menuName = "Scriptable Object/Specimen/Map Group")]
public class MapGroupSO : ScriptableObject
{
    [SerializeField] Sprite lowResSprite;
    public Sprite LowResSprite => lowResSprite;
    
    [SerializeField] Sprite medResSprite;
    public Sprite MedResSprite => medResSprite;
    
    [SerializeField] Sprite highResSprite;
    public Sprite HighResSprite => highResSprite;
}