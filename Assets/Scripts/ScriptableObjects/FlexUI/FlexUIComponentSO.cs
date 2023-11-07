using UnityEngine;

[CreateAssetMenu(fileName = "FlexUIComponentSO", menuName = "Scriptable Object/FlexUI/FlexUI Component")]
public class FlexUIComponentSO : FlexUIDataSO
{
    [SerializeField] FlexUIDataSO[] dataTypes;
    
    public override void UpdatePropertyValue(Component component)
    {
        foreach (var dataType in dataTypes)
        {
            dataType.UpdatePropertyValue(component);
        }
    }
}