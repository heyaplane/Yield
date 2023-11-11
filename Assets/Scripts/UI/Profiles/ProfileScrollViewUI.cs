public class ProfileScrollViewUI : BaseScrollView
{
    protected override string[] GetItemNames() => SaveManager.Instance.GetSavedProfileNames();
}
