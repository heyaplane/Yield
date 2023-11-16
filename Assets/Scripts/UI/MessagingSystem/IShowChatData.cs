public interface IShowChatData
{
    HighlightOnClick HighlightOnClick { get; }
    void InitializeMessageData(IChatData chatData);
    void ActionOnHighlight(HighlightOnClick highlightOnClick);
}
