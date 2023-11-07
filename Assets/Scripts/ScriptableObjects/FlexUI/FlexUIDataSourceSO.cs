using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "FlexUIDataSourceSO", menuName = "Scriptable Object/FlexUI/FlexUI Data Source")]
public class FlexUIDataSourceSO : ScriptableObject
{
    [Header("File path to update FlexUITypeSOs")] 
    [SerializeField] string path;
    public string Path => path;

    [SerializeField] FlexUIFontAssetData[] fontAssets;
    [SerializeField] FlexUIFontSizeData[] fontSizes;
    [SerializeField] FlexUIColorData[] colors;
    [SerializeField] FlexUISpriteData[] sprites;
    [SerializeField] FlexUISpriteStateData[] spriteStates;

    Dictionary<FlexUIFontAssetSO, TMP_FontAsset> fontAssetDictionary;
    Dictionary<FlexUIFontSizeSO, float> fontSizeDictionary;
    Dictionary<FlexUIColorSO, Color> colorDictionary;
    Dictionary<FlexUISpriteSO, Sprite> spriteDictionary;
    Dictionary<FlexUISpriteStateSO, SpriteState> spriteStateDictionary;

    void OnValidate()
    {
        UpdateSourceDictionaries();
    }

    public void UpdateSourceDictionaries()
    {
        fontAssetDictionary = fontAssets?.ToDictionary(item => item.FontAssetSO, item => item.FontAsset);
        fontSizeDictionary = fontSizes?.ToDictionary(item => item.FontSizeSO, item => item.TextSize);
        colorDictionary = colors?.ToDictionary(item => item.ColorSO, item => item.Color);
        spriteDictionary = sprites?.ToDictionary(item => item.SpriteSO, item => item.Sprite);
        spriteStateDictionary = spriteStates?.ToDictionary(item => item.SpriteStateSO, item => item.SpriteState);
    }

    public TMP_FontAsset GetFontAsset(FlexUIFontAssetSO dataSO)
    {
        if (fontAssetDictionary == null)
        {
            UpdateSourceDictionaries();
            Assert.IsNotNull(fontAssetDictionary);
        }

        return fontAssetDictionary.TryGetValue(dataSO, out TMP_FontAsset value) ? value : null;
    }
    
    public float GetFontSize(FlexUIFontSizeSO dataSO)
    {
        if (fontSizeDictionary == null)
        {
            UpdateSourceDictionaries();
            Assert.IsNotNull(fontSizeDictionary);
        }
        
        return fontSizeDictionary.TryGetValue(dataSO, out float value) ? value : 0;
    }

    public Color GetColor(FlexUIColorSO dataSO)
    {
        if (colorDictionary == null)
        {
            UpdateSourceDictionaries();
            Assert.IsNotNull(colorDictionary);
        }

        return colorDictionary.TryGetValue(dataSO, out Color value) ? value : default;
    }

    public Sprite GetSprite(FlexUISpriteSO dataSO)
    {
        if (spriteDictionary == null)
        {
            UpdateSourceDictionaries();
            Assert.IsNotNull(spriteDictionary);
        }

        return spriteDictionary.TryGetValue(dataSO, out Sprite value) ? value : null;
    }
    
    public SpriteState GetSpriteState(FlexUISpriteStateSO dataSO)
    {
        if (spriteStateDictionary == null)
        {
            UpdateSourceDictionaries();
            Assert.IsNotNull(spriteStateDictionary);
        }

        return spriteStateDictionary.TryGetValue(dataSO, out SpriteState value) ? value : default;
    }
}

[Serializable]
public struct FlexUIFontAssetData
{
    public FlexUIFontAssetSO FontAssetSO;
    public TMP_FontAsset FontAsset;
}

[Serializable]
public struct FlexUIFontSizeData
{
    public FlexUIFontSizeSO FontSizeSO;
    public float TextSize;
}

[Serializable]
public struct FlexUIColorData
{
    public FlexUIColorSO ColorSO;
    public Color Color;
}

[Serializable]
public struct FlexUISpriteData
{
    public FlexUISpriteSO SpriteSO;
    public Sprite Sprite;
}

[Serializable]
public struct FlexUISpriteStateData
{
    public FlexUISpriteStateSO SpriteStateSO;
    public SpriteState SpriteState;
}
