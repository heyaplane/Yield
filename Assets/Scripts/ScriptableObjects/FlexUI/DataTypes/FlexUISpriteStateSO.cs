using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "FlexUISpriteStateSO", menuName = "Scriptable Object/FlexUI/Data/Sprite State")]
public class FlexUISpriteStateSO : FlexUIDataSO
{
    public override void UpdatePropertyValue(Component component)
    {
        var spriteState = DataSource.GetSpriteState(this);

        if (component is Button button)
        {
            button.spriteState = spriteState;
        }
    }
}