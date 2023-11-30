using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaferManager : SingletonMonobehaviour<WaferManager>
{
    [SerializeField] List<WaferDataSO> wafers;

    Dictionary<string, WaferDataSO> waferLookup;

    WaferDataSO activeWafer;

    public WaferDataSO ActiveWafer
    {
        get => activeWafer;
        set
        {
            activeWafer = value;
            wafersGivenToPlayer.Add(value);
        }
    }

    HashSet<WaferDataSO> wafersGivenToPlayer;
    WaferLayout waferLayout;

    void OnEnable()
    {
        wafersGivenToPlayer = new HashSet<WaferDataSO>();
        waferLookup = wafers.ToDictionary(x => x.WaferName, x => x);
    }

    void Start()
    {
        waferLayout = new WaferLayout(ActiveWafer.WaferMap.ChunkDimSize, ActiveWafer.WaferMap.SectionDimSize);
    }

    public List<WaferDataSO> GetSamplesWithoutReports()
    {
        var reports = FileSystemManager.Instance.FindDirectoryInRoot("Reports")?.DirectoryFiles.OfType<VirtualReport>().Select(x => x.WaferMap).ToList();
        if (reports == null || reports.Count == 0) return wafersGivenToPlayer.ToList();
        
        return wafersGivenToPlayer.Where(x => !reports.Contains(x.WaferMap)).ToList();
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

    public int GetDeviceYield() => activeWafer.DeviceYield;
}
