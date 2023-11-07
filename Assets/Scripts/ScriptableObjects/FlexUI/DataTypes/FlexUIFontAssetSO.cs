using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "FlexUIFontAssetSO", menuName = "Scriptable Object/FlexUI/Data/Font Asset")]
public class FlexUIFontAssetSO : FlexUIDataSO
{
    public override void UpdatePropertyValue(Component component)
    {
        var fontAsset = DataSource.GetFontAsset(this);

        if (component is TextMeshProUGUI tmPro && fontAsset != null)
        {
            tmPro.font = fontAsset;
        }
    }
}