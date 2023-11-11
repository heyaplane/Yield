using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadToJson<T>
{
    const string encryptionCodeWord = "egret";
    bool useEncryption;
    BasicEncrypt encrypter;
    JsonFormatter<T> formatter;
    
    public SaveLoadToJson(JsonFormatter<T> formatter, bool useEncryption)
    {
        this.formatter = formatter;
        this.useEncryption = useEncryption;
        
        encrypter = new BasicEncrypt(encryptionCodeWord);
    }
    
    public string SaveIO(T data, string fullPath)
    {
        try
        {
            string dataToStore = formatter.Serialize(data);

            if (useEncryption)
                dataToStore = encrypter.EncryptDecrypt(dataToStore);

            using var stream = new FileStream(fullPath, FileMode.Create);
            using var writer = new StreamWriter(stream);

            writer.Write(dataToStore);
            return dataToStore;
        }
        catch (Exception e)
        {
            Debug.LogError("Error while saving data: " + fullPath + "\n" + e);
            return null;
        }
    }

    public T LoadIO(string fullPath)
    {
        try
        {
            using var stream = new FileStream(fullPath, FileMode.Open);
            using var reader = new StreamReader(stream);
            string dataToLoad = reader.ReadToEnd();

            if (useEncryption)
                dataToLoad = encrypter.EncryptDecrypt(dataToLoad);

            return formatter.Deserialize(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError("Error while trying to load file at path: " + fullPath + "\n" + e);
            return default;
        }
    }
}