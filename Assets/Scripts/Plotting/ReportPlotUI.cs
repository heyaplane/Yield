using System;
using ScottPlot;
using ScottPlot.Plottables;
using SkiaSharp;
using UnityEngine;
using UnityEngine.UI;
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

    public void AddHistogramToPlot(double[] data, float dutyCycle)
    {
        var histogramData = new HistogramBarData(data);
        var barPlot = plot.Add.Bar(histogramData.Bars);
        barPlot.Padding = 0.5f - histogramData.BinSize * dutyCycle;
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
