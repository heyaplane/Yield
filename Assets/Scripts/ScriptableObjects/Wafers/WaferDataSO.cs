using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "WaferDataSO", menuName = "Scriptable Object/Wafer/Wafer Data")]
public class WaferDataSO : ScriptableObject
{
    [SerializeField] bool shouldRandomGenerateName;
    
    [SerializeField] string waferName;
    public string WaferName => shouldRandomGenerateName ? GetRandomWaferName() : waferName;

    [SerializeField] WaferMapSO waferMap;
    public WaferMapSO WaferMap => waferMap;

    string GetRandomWaferName() => GUID.Generate().ToString();

}