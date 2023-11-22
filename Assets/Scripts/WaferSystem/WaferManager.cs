using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaferManager : SingletonMonobehaviour<WaferManager>
{
    [SerializeField] List<WaferDataSO> wafers;

    Dictionary<string, WaferDataSO> waferLookup;
    
    public WaferDataSO ActiveWafer { get; set; }
    WaferLayout waferLayout;

    void OnEnable()
    {
        waferLookup = wafers.ToDictionary(x => x.WaferName, x => x);
        ActiveWafer = wafers[0];
        
        waferLayout = new WaferLayout(ActiveWafer.WaferMap.ChunkDimSize, ActiveWafer.WaferMap.SectionDimSize);
    }

    public List<WaferDataSO> GetSamplesWithoutReports()
    {
        var reports = FileSystemManager.Instance.FindDirectoryInRoot("Reports")?.DirectoryFiles.OfType<VirtualReport>().Select(x => x.WaferMap).ToList();
        if (reports == null || reports.Count == 0) return wafers;
        
        return wafers.Where(x => !reports.Contains(x.WaferMap)).ToList();
    }

    public WaferDataSO GetWaferDataFromName(string waferName)
    {
        if (waferLookup.TryGetValue(waferName, out var waferDataSO)) return waferDataSO;
        
        Debug.LogError("Cannot find wafer name!");
        return null;
    }

    public string GetSectionLocationAsStringFromChunk(ChunkCoordinate chunkCoordinate)
    {
        var location = waferLayout.GetWaferSectionLocationFromChunk(chunkCoordinate);
        return $"{location.x},{location.y}";
    }
}
