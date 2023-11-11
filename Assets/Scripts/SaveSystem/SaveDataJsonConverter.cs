using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class SaveDataJsonConverter : JsonConverter<SaveData>
{
    public override SaveData ReadJson(JsonReader reader, Type objectType, SaveData existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        SaveData saveData = new SaveData();
        JObject jObject = JObject.Load(reader);

        foreach (var property in jObject.Properties())
        {
            string key = property.Name;
            JObject itemObject = (JObject)property.Value;

            string typeString = itemObject["Type"]?.ToObject<string>(serializer);
            if (typeString == null) continue;
            
            Type valueType = Type.GetType(typeString);
            JToken valueToken = itemObject["Value"];
            if (valueToken == null || valueType == null) continue;
            
            object value = valueToken.ToObject(valueType, serializer);
            saveData[key] = value;
        }

        return saveData;
    }

    public override void WriteJson(JsonWriter writer, SaveData value, JsonSerializer serializer)
    {
        JObject jObject = new JObject();
        foreach (var keyValuePair in value)
        {
            JObject itemObject = new JObject
            {
                { "Type", keyValuePair.Value.GetType().AssemblyQualifiedName },
                { "Value", JToken.FromObject(keyValuePair.Value, serializer) }
            };

            jObject.Add(keyValuePair.Key, itemObject);
        }
        jObject.WriteTo(writer);
    }
}

