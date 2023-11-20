using UnityEngine;

[CreateAssetMenu(fileName = "LMWaferSO", menuName = "Scriptable Object/Wafer/Wafer Map Group")]
public class WaferMapGroupSO : ScriptableObject
{
    [SerializeField] Sprite lowResSprite;
    public Sprite LowResSprite => lowResSprite;
    
    [SerializeField] Sprite medResSprite;
    public Sprite MedResSprite => medResSprite;
    
    [SerializeField] Sprite highResSprite;
    public Sprite HighResSprite => highResSprite;
}