using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaferManager : SingletonMonobehaviour<WaferManager>
{
    [SerializeField] List<WaferDataSO> wafers;

    Dictionary<string, WaferMapSO> waferLookup;

    void OnEnable()
    {
        waferLookup = wafers.ToDictionary(x => x.WaferName, x => x.WaferMap);
    }

    public List<WaferDataSO> GetSamplesWithoutReports()
    {
        var reports = FileSystemManager.Instance.FindDirectoryInRoot("Reports")?.DirectoryFiles.OfType<VirtualReport>().Select(x => x.WaferMap).ToList();
        if (reports == null || reports.Count == 0) return wafers;
        
        return wafers.Where(x => !reports.Contains(x.WaferMap)).ToList();
    }

    public WaferMapSO GetWaferMapFromName(string waferName)
    {
        if (waferLookup.TryGetValue(waferName, out var waferMapSO)) return waferMapSO;
        
        Debug.LogError("Cannot find wafer name!");
        return null;
    }
}
