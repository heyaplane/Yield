using System;
using System.Collections.Generic;
using System.Linq;
using ScottPlot.Plottables;

public class KDEPlotData
{
    public List<Bar> Bars { get; }
    public (double[], double[]) KDEPoints { get; }
    
    public KDEPlotData(double[] data, double stDev)
    {
        Bars = GenerateBars(data);
        KDEPoints = GenerateKDEPoints(data, stDev);
    }

    List<Bar> GenerateBars(double[] data)
    {
        return data.Select(dataPoint => new Bar {Position = dataPoint, Value = 0.1, ValueBase = 0}).ToList();
    }

    (double[], double[]) GenerateKDEPoints(double[] data, double stDev)
    {
        double min = data.Min();
        double max = data.Max();
        double kernelStDev = Math.Pow(4 * Math.Pow(stDev, 5) / (3 * data.Length), 0.2);
        
        double plotMin = min - 3 * kernelStDev;
        double plotMax = max + 3 * kernelStDev;
        double interval = (plotMax - plotMin) / 100;

        List<double> xPointsList = new List<double>();
        List<double> yPointsList = new List<double>();

        for (double x = plotMin; x < plotMax; x += interval)
        {
            xPointsList.Add(x);
            yPointsList.Add(0);
            
            foreach (double dataPoint in data)
            {
                yPointsList[^1] += MathHelpers.GaussianPDF(x, dataPoint, kernelStDev);
            }
        }

        double[] xPoints = xPointsList.ToArray();
        return (xPoints, MathHelpers.NormalizeDistribution(xPoints, yPointsList.ToArray()));
    }
}
