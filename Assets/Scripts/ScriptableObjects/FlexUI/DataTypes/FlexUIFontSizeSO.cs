using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "FlexUIFontSizeSO", menuName = "Scriptable Object/FlexUI/Data/Font Size")]
public class FlexUIFontSizeSO : FlexUIDataSO
{
    public override void UpdatePropertyValue(Component component)
    {
        float fontSize = DataSource.GetFontSize(this);

        if (component is TextMeshProUGUI tmPro)
        {
            tmPro.fontSize = fontSize;
        }
    }
}