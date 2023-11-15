using System;
using System.Collections.Generic;
using System.Linq;
using ScottPlot.Plottables;
using ScottPlot.Statistics;
using UnityEngine;

public class HistogramBarData
{
    Histogram histogram;

    public List<Bar> Bars { get; }
    public double BinSize => histogram.BinSize;
    
    public HistogramBarData(double[] data)
    {
        int numBins = Mathf.Min(Mathf.RoundToInt(2 * Mathf.Pow(data.Length, 1 / 3f)), 1);
        histogram = new Histogram(data.Min(), data.Max(), numBins);
        histogram.AddRange(data);
        Bars = GenerateBars();
    }

    List<Bar> GenerateBars() => histogram.Counts.Select((t, i) => new Bar(histogram.BinCenters[i], t)).ToList();
}
