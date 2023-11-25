using System;
using System.Collections.Generic;

public static class MathHelpers
{
    public static (double[], double[]) GenerateGaussianDistributionPoints(double mean, double stDev, double interval = -1, int stDevRange = -1)
    {
        List<double> xPoints = new List<double>();
        List<double> yPoints = new List<double>();

        if (stDevRange < 0)
            stDevRange = 3;
        
        double start = mean - stDevRange * stDev;
        double end = mean + stDevRange * stDev;

        if (interval < 0)
            interval = (end - start) / 100;

        for (double x = start; x <= end; x += interval)
        {
            xPoints.Add(x);
            yPoints.Add(GaussianPDF(x, mean, stDev));
        }

        return (xPoints.ToArray(), yPoints.ToArray());
    }

    public static double GaussianPDF(double x, double mean, double stDev)
    {
        double exponent = -0.5f * Math.Pow((x - mean) / stDev, 2);
        return (1 / (stDev * Math.Sqrt(2 * Math.PI))) * Math.Exp(exponent);
    }

    public static double[] NormalizeDistribution(double[] x, double[] y)
    {
        double[] normalized = new double[y.Length];
        double area = IntegrateByTrapezoidalRule(x, y);

        for (int i = 0; i < y.Length; i++)
        {
            normalized[i] = y[i] / area;
        }

        return normalized;
    }

    static double IntegrateByTrapezoidalRule(double[] x, double[] y)
    {
        double area = 0.0;

        for (int i = 0; i < x.Length - 1; i++)
        {
            double width = x[i + 1] - x[i];
            double height = (y[i] + y[i + 1]) / 2;
            area += width * height;
        }

        return area;
    }
}
