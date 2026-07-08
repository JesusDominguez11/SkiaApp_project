using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaApp;

public partial class DashboardPage : ContentPage
{
    private float _progress = 0;
    private const float AnimationSpeed = 0.01f;

    public DashboardPage()
	{
		InitializeComponent();

        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            _progress += AnimationSpeed;

            if (_progress > 1)
                _progress = 1;

            canvasView.InvalidateSurface();

            return _progress < 1;
        });
    }



    private void Canvas_PaintSurface(
    object? sender,
    SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        using var paint = new SKPaint
        {
            Color = SKColors.ForestGreen,
            StrokeWidth = 12,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        float centerX = e.Info.Width / 2f;

        float startY = e.Info.Height - 80;
        float endY = e.Info.Height / 2f;

        float currentY = startY - (startY - endY) * _progress;

        var path = new SKPath();

        path.MoveTo(centerX, startY);

        path.QuadTo(
            centerX + 40,
            (startY + endY) / 2,
            centerX,
            currentY);

        var measure = new SKPathMeasure(path);

        var partialPath = new SKPath();

        measure.GetSegment(
            0,
            measure.Length * _progress,
            partialPath,
            true);

        canvas.DrawPath(partialPath, paint);
    }
}