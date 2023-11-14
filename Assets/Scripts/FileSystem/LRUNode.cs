public class LRUNode
{
    public string Key { get; set; }
    public object Value { get; set; }
    public LRUNode Prev { get; set; }
    public LRUNode Next { get; set; }
}
