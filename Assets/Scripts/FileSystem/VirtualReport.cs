using System;

public class VirtualReport : IVirtualFile
{
    public string FileName { get; set; }
    public DateTime CreationDateTime { get; }
    public DateTime LastModifiedDateTime { get; set; }
    public int FileSize { get; }
    public ReportData ReportData { get; }

    public VirtualReport(string fileName, ReportData reportData)
    {
        FileName = fileName;
        CreationDateTime = DateTime.Now;
        LastModifiedDateTime = DateTime.Now;
        ReportData = reportData;
        FileSize = CalculateReportSize();
    }

    public VirtualReport(SerializedFile serializedFile)
    {
        FileName = serializedFile.FileName;
        CreationDateTime = serializedFile.CreationDateTime;
        LastModifiedDateTime = serializedFile.LastModifiedDateTime;
        FileSize = FileSize;
        ReportData = serializedFile.AdditionalData as ReportData;
    }

    // Mean and StDev both take 4 bytes (floats), and each double is 8 bytes
    
    int CalculateReportSize() => 2 * 4 + 8 * ReportData.Measurements.Length;

    public SerializedFile GetSerializableFile()
    {
        return new SerializedFile
        {
            FileName = FileName,
            CreationDateTime = CreationDateTime,
            LastModifiedDateTime = LastModifiedDateTime,
            FileSize = FileSize,
            FileType = TypeOfFile.Report,
            AdditionalData = ReportData
        };
    }
    
    public void SavePersistentFile() { }
    public void DestroyUnsavedPersistentFiles() { }
}
