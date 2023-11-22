using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirtualReport : IVirtualFile
{
    public string FileName { get; set; }
    public DateTime CreationDateTime { get; }
    public DateTime LastModifiedDateTime { get; set; }
    public int FileSize { get; }
    public WaferDataSO WaferData { get; }
    public WaferMapSO WaferMap => WaferData.WaferMap;
    public string WaferName => WaferData.WaferName;

    Dictionary<string, List<ReportEntry>> reportEntriesLookup;
    public bool TryGetReportEntry(string sectionLocation, string featureName, out ReportEntry reportEntry)
    {
        if (reportEntriesLookup.TryGetValue(sectionLocation, out var reportEntries))
        {
            reportEntry = reportEntries.FirstOrDefault(x => x.FeatureName == featureName);
            return true;
        }

        reportEntry = null;
        return false;
    }

    public VirtualReport(string fileName, WaferDataSO waferData)
    {
        reportEntriesLookup = new Dictionary<string, List<ReportEntry>>();
        
        FileName = fileName;
        CreationDateTime = DateTime.Now;
        LastModifiedDateTime = DateTime.Now;
        FileSize = CalculateReportSize();
        WaferData = waferData;

        GenerateReportEntries();
    }

    public VirtualReport(SerializedFile serializedFile)
    {
        FileName = serializedFile.FileName;
        CreationDateTime = serializedFile.CreationDateTime;
        LastModifiedDateTime = serializedFile.LastModifiedDateTime;
        FileSize = FileSize;
        reportEntriesLookup = (Dictionary<string, List<ReportEntry>>) serializedFile.AdditionalData;
    }

    // Mean and StDev both take 4 bytes (floats), and each double is 8 bytes
    
    int CalculateReportSize() => 2 * 4 + 8 * reportEntriesLookup.Sum(x => x.Value.Sum(x => x.Measurements.Length));

    public SerializedFile GetSerializableFile()
    {
        return new SerializedFile
        {
            FileName = FileName,
            CreationDateTime = CreationDateTime,
            LastModifiedDateTime = LastModifiedDateTime,
            FileSize = FileSize,
            FileType = TypeOfFile.Report,
            AdditionalData = reportEntriesLookup
        };
    }

    void GenerateReportEntries()
    {
        var sectionDirectories = FileSystemManager.Instance.FindDirectoryInRoot(WaferData.WaferName).DirectoryFiles;

        foreach (var virtualFile in sectionDirectories)
        {
            if (virtualFile is not VirtualDirectory directory) continue;
            var sectionLocation = new Vector2Int(int.Parse(directory.FileName[0].ToString()), int.Parse(directory.FileName[^1].ToString()));
            var sectionData = WaferMap.GetSectionDataFromLocation(sectionLocation);
            reportEntriesLookup[directory.FileName] = new List<ReportEntry>();
            foreach (var data in sectionData)
            {
                reportEntriesLookup[directory.FileName].Add(new ReportEntry(WaferName, directory.FileName, data.Feature.FeatureName, ReportEntryState.DataExist));
            }
        }
    }
    
    public void SavePersistentFile() { }
    public void DestroyUnsavedPersistentFiles() { }
}

[Serializable]
public class ReportEntry
{
    public string WaferName;
    public string SectionName;
    public string FeatureName;
    public double[] Measurements { get; set; } 
    public float Mean { get; set; }
    public float StDev { get; set; }
    public ReportEntryState State { get; }

    public ReportEntry(string waferName, string sectionName, string featureName, ReportEntryState startingState)
    {
        WaferName = waferName;
        SectionName = sectionName;
        FeatureName = featureName;
        State = startingState;
    }
}

public enum ReportEntryState
{
    Default, Selected, DataExist, Pass, Fail
}
