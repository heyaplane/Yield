public class BasicEncrypt
{
    readonly string codeWord;
    
    public BasicEncrypt(string codeWord)
    {
        this.codeWord = codeWord;
    }
    
    public string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char) (data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifiedData;
    }
}