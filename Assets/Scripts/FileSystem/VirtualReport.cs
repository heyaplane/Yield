using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
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
    public ErrorType ProcessRecommendation { get; set; }

    Dictionary<string, List<ReportEntry>> reportEntriesLookup;
    
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
        var additionalData = (serializedFile.AdditionalData as JObject)?.ToObject<SaveData>();
        reportEntriesLookup = SaveSystemHelpers.UnpackJObject(additionalData["Entries"] as JObject) as Dictionary<string, List<ReportEntry>>;
        WaferData = WaferManager.Instance.GetWaferDataFromName((string) SaveSystemHelpers.UnpackJObject(additionalData["WaferData"] as JObject));
        ProcessRecommendation = (ErrorType) SaveSystemHelpers.UnpackJObject(additionalData["ProcessRec"] as JObject);
    }

    public bool TryGetReportEntry(string sectionLocation, string featureName, out ReportEntry reportEntry)
    {
        reportEntry = null;
        
        if (reportEntriesLookup.TryGetValue(sectionLocation, out var reportEntries))
        {
            reportEntry = reportEntries.FirstOrDefault(x => x.FeatureName == featureName);
        }

        return reportEntry != null;
    }
    
    public void AddReportEntry(string sectionName, ReportEntry reportEntry) => reportEntriesLookup[sectionName].Add(reportEntry);

    public void ReplaceReportEntry(string sectionName, ReportEntry reportEntry)
    {
        if (reportEntriesLookup.TryGetValue(sectionName, out var reportEntries))
        {
            var existingReport = reportEntries.FirstOrDefault(x => x.FeatureName == reportEntry.FeatureName);
            if (existingReport != null)
            {
                reportEntries[reportEntries.IndexOf(existingReport)] = reportEntry;
                return;
            }
            
            Debug.LogWarning($"Problem replacing existing report. Adding new report entry for {sectionName}");
            reportEntries.Add(reportEntry);
            return;
        }
        
        Debug.LogError("Couldn't find section in report lookup.");
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
            AdditionalData = new SaveData
            {
                {"Entries", reportEntriesLookup},
                {"WaferData", WaferData.WaferName},
                {"ProcessRec", ProcessRecommendation}
            }
        };
    }

    void GenerateReportEntries()
    {
        var waferDirectory = FileSystemManager.Instance.FindDirectoryInRoot(WaferData.WaferName);
        if (waferDirectory == null) return;
        
        var sectionDirectories = waferDirectory.DirectoryFiles;

        foreach (var virtualFile in sectionDirectories)
        {
            if (virtualFile is not VirtualDirectory directory) continue;
            var sectionLocation = new Vector2Int(int.Parse(directory.FileName[0].ToString()), int.Parse(directory.FileName[^1].ToString()));
            var sectionData = WaferMap.GetSectionDataFromLocation(sectionLocation);
            reportEntriesLookup[directory.FileName] = new List<ReportEntry>();
            foreach (var data in sectionData)
            {
                var reportEntry = new ReportEntry(WaferName, directory.FileName, data.Feature.FeatureName, ReportEntryState.DataExist);
                AddReportEntry(directory.FileName, reportEntry);
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
    public ReportEntryState State { get; set; }

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
