using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "FlexUIColorSO", menuName = "Scriptable Object/FlexUI/Data/Color")]
public class FlexUIColorSO : FlexUIDataSO
{
    public override void UpdatePropertyValue(Component component)
    {
        var color = DataSource.GetColor(this);

        switch (component)
        {
            case TextMeshProUGUI tmPro:
                tmPro.color = color;
                return;
            case Image image:
                image.color = color;
                break;
        }
    }
}