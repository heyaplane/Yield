using System;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using MathNet.Numerics.Statistics;
using ScottPlot;
using ScottPlot.Plottables;
using SkiaSharp;
using UnityEngine;
using UnityEngine.UI;
using Color = ScottPlot.Color;
using Image = ScottPlot.Image;

public class ReportPlotUI : MonoBehaviour
{
    [SerializeField] RawImage plotImage;
    [SerializeField] RectTransform rectTransform;

    Texture2D texture;
    Plot plot;
    BarPlot bar;
    SKSurface surf;
    SKImageInfo imageInfo;

    void OnEnable()
    {
        plot = new Plot();
        imageInfo = new SKImageInfo((int) rectTransform.rect.width, (int) rectTransform.rect.height, SKColorType.Rgba8888, SKAlphaType.Premul);
        surf = SKSurface.Create(imageInfo);
        texture = new Texture2D(imageInfo.Width, imageInfo.Height);
    }

    public void ClearPlot()
    {
        plot.Clear();
        RenderPlot();
    }

    public void AddHistogramToPlot(double[] data, float dutyCycle)
    {
        var histogramData = new HistogramBarData(data);
        var barPlot = plot.Add.Bar(histogramData.Bars);
        barPlot.Padding = (1 - dutyCycle);
        RenderPlot();
    }

    public void AddGaussianToPlot(float mean, float stDev, double interval = -1, int stDevRange = -1)
    {
        (double[] xPoints, double[] yPoints) = MathHelpers.GenerateGaussianDistributionPoints(mean, stDev, interval, stDevRange);
        double[] baselineYPoints = new double[yPoints.Length];
        var gaussian = plot.Add.FillY(xPoints, baselineYPoints, yPoints);
        //gaussian.FillStyle.Color = Color.FromARGB(ColorHelper.GetColorARGB(0.5f, 0.5f, 0.5f, 0.5f));
        gaussian.FillStyle.Color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        RenderPlot();
    }

    public void AddKDEToPlot(double[] data, double stDev)
    {
        var kdeData = new KDEPlotData(data, stDev);
        
        (double[] xPoints, double[] yPoints) = kdeData.KDEPoints;
        double[] baselineYPoints = new double[yPoints.Length];
        var kde = plot.Add.FillY(xPoints, baselineYPoints, yPoints);
        kde.FillStyle.Color = new Color(1, 0, 0, 0.5f);

        var barSeries = new BarSeries();
        barSeries.Bars = kdeData.Bars;
        barSeries.Color = new Color(0, 0, 0, 1f);
        var barPlot = plot.Add.Bar(new List<BarSeries>{barSeries});
        barPlot.Padding = 0.5;
        
        RenderPlot();
    }

    void RenderPlot()
    {
        plot.AutoScale();
        plot.Render(surf.Canvas, imageInfo.Width, imageInfo.Height);
        var snapshot = new Image(surf.Snapshot());
        texture.LoadImage(snapshot.GetImageBytes());
        plotImage.texture = texture;
    }
}
