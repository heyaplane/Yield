using UnityEngine;

public class SaveGameScrollViewUI : BaseScrollView
{
    protected override string[] GetItemNames()
    {
        return SaveManager.Instance.GetSaveGameNames();
    }
}
