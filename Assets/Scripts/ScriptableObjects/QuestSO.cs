using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "Scriptable Object/Quest")]
public class QuestSO : ScriptableObject
{
    [SerializeField] WaferDataSO passingWafer;
    public WaferDataSO PassingWafer => passingWafer;

    [SerializeField] WaferDataSO uniformFailWafer;
    public WaferDataSO UniformFailWafer => uniformFailWafer;

    [SerializeField] WaferDataSO radialFailInsideWafer;
    public WaferDataSO RadialFailInsideWafer => radialFailInsideWafer;

    [SerializeField] WaferDataSO radialFailOutsideWafer;
    public WaferDataSO RadialFailOutsideWafer => radialFailOutsideWafer;

    public WaferDataSO StartingWafer => uniformFailWafer;
}