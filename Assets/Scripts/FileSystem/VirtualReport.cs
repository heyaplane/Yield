using System;
using System.Collections.Generic;
using System.Linq;

public class VirtualReport : IVirtualFile
{
    public string FileName { get; set; }
    public DateTime CreationDateTime { get; }
    public DateTime LastModifiedDateTime { get; set; }
    public int FileSize { get; }
    public List<ReportEntry> ReportEntries { get; }
    public WaferMapSO WaferMap { get; }

    public VirtualReport(string fileName, WaferMapSO waferMap)
    {
        ReportEntries = new List<ReportEntry>();
        
        FileName = fileName;
        CreationDateTime = DateTime.Now;
        LastModifiedDateTime = DateTime.Now;
        FileSize = CalculateReportSize();
        WaferMap = waferMap;
    }

    public VirtualReport(SerializedFile serializedFile)
    {
        FileName = serializedFile.FileName;
        CreationDateTime = serializedFile.CreationDateTime;
        LastModifiedDateTime = serializedFile.LastModifiedDateTime;
        FileSize = FileSize;
        ReportEntries = (List<ReportEntry>) serializedFile.AdditionalData;
    }

    // Mean and StDev both take 4 bytes (floats), and each double is 8 bytes
    
    int CalculateReportSize() => 2 * 4 + 8 * ReportEntries.Sum(x => x.Measurements.Length);

    public SerializedFile GetSerializableFile()
    {
        return new SerializedFile
        {
            FileName = FileName,
            CreationDateTime = CreationDateTime,
            LastModifiedDateTime = LastModifiedDateTime,
            FileSize = FileSize,
            FileType = TypeOfFile.Report,
            AdditionalData = ReportEntries
        };
    }
    
    public void SavePersistentFile() { }
    public void DestroyUnsavedPersistentFiles() { }
}
