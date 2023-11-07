using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "FlexUISpriteSO", menuName = "Scriptable Object/FlexUI/Data/Sprite")]
public class FlexUISpriteSO : FlexUIDataSO
{
    public override void UpdatePropertyValue(Component component)
    {
        var sprite = DataSource.GetSprite(this);

        if (component is Image image && sprite != null)
        {
            image.sprite = sprite;
        }
    }
}