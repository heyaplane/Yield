using System.Collections.Generic;

public class SaveData : Dictionary<string, object>
{
    public SaveData() : base() { }

    public SaveData(Dictionary<string, object> dictionary) : base(dictionary) { } 
}
