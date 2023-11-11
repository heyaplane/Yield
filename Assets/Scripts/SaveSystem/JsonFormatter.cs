using Newtonsoft.Json;

public class JsonFormatter<T>
{
    readonly JsonConverter<T> jsonConverter;
    readonly Formatting formatting;

    public JsonFormatter(JsonConverter<T> converter, Formatting formatting)
    {
        jsonConverter = converter;
        this.formatting = formatting;
    }

    public string Serialize(T data)
    {
        if (jsonConverter != null)
            return JsonConvert.SerializeObject(data, formatting, jsonConverter);
        return JsonConvert.SerializeObject(data, formatting);
    }

    public T Deserialize(string data)
    {
        if (jsonConverter != null)
            return JsonConvert.DeserializeObject<T>(data, jsonConverter);
        return JsonConvert.DeserializeObject<T>(data);
    }
}