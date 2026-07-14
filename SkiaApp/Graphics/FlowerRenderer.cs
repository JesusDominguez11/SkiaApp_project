using SkiaSharp;
using Microsoft.Maui.Dispatching;

namespace SkiaApp.Graphics;

public class FlowerRenderer : IDrawingRenderer
{
    private enum FlowerState
    {
        Growing,
        Wind
    }

    private FlowerState _state = FlowerState.Growing;

    /// <summary>
    /// Progreso de la animación inicial (0-1)
    /// </summary>
    private float _growth = 0;

    /// <summary>
    /// Tiempo para la animación del viento
    /// </summary>
    private float _windTime = 0;




    public string Name => "flower";
    public event Action? Updated;
    private double _progress;
    private IDispatcherTimer? _timer;









    public void StartAnimation()
    {
        _growth = 0;
        _windTime = 0;
        _state = FlowerState.Growing;

        _timer?.Stop();
        _timer = Dispatcher.GetForCurrentThread().CreateTimer();
        _timer.Interval = TimeSpan.FromMicroseconds(16);


        _timer.Tick += (s, e) =>
        {
            if (_state == FlowerState.Growing)
            {
                _growth += 0.01f;


                if (_growth >= 1f)
                {
                    _growth = 1f;

                    _state = FlowerState.Wind;
                }
            }
            else
            {
                _windTime += 0.05f;
            }

            Updated?.Invoke();
        };


        _timer.Start();
    }



    public void StopAnimation()
    {
        _timer?.Stop();
    }

    public void Draw(
    SKCanvas canvas,
    int width,
    int height)
    {
        canvas.Clear(SKColors.Black);

        float centerX = width / 2f;

        float groundY = height - 80;

        float flowerY = height / 2f;

        //------------------------------------------
        // VIENTO
        //------------------------------------------

        float windAngle = 0;

        if (_state == FlowerState.Wind)
            windAngle = (float)Math.Sin(_windTime) * 5f;

        //------------------------------------------
        // TALLO
        //------------------------------------------

        using var stemPaint = new SKPaint
        {
            Color = SKColors.ForestGreen,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        float stemProgress = _growth;

        float currentY =
            groundY -
            (groundY - flowerY) * stemProgress;

        var stem = new SKPath();

        stem.MoveTo(centerX, groundY);

        stem.QuadTo(
            centerX + windAngle * 2,
            (groundY + flowerY) / 2,
            centerX + windAngle,
            currentY);

        canvas.DrawPath(stem, stemPaint);

        //------------------------------------------
        // HOJAS
        //------------------------------------------

        if (_growth > 0.35f)
        {
            float leafProgress =
                (_growth - 0.35f) / 0.65f;

            leafProgress = Math.Clamp(
                leafProgress,
                0,
                1);

            using var leafPaint = new SKPaint
            {
                Color = SKColors.Green,
                IsAntialias = true
            };

            float leafSize = 45 * leafProgress;

            canvas.Save();

            canvas.RotateDegrees(
                -35 + windAngle,
                centerX,
                groundY - 170);

            canvas.DrawOval(
                new SKRect(
                    centerX - leafSize,
                    groundY - 200,
                    centerX,
                    groundY - 170),
                leafPaint);

            canvas.Restore();

            canvas.Save();

            canvas.RotateDegrees(
                35 + windAngle,
                centerX,
                groundY - 270);

            canvas.DrawOval(
                new SKRect(
                    centerX,
                    groundY - 300,
                    centerX + leafSize,
                    groundY - 270),
                leafPaint);

            canvas.Restore();
        }

        //------------------------------------------
        // FLOR
        //------------------------------------------

        if (_growth > 0.70f)
        {
            float flowerProgress =
                (_growth - 0.70f) / 0.30f;

            flowerProgress =
                Math.Clamp(
                    flowerProgress,
                    0,
                    1);

            float radius =
                30 * flowerProgress;

            float petalDistance =
                radius * 1.6f;

            canvas.Save();

            canvas.RotateDegrees(
                windAngle,
                centerX,
                flowerY);

            using var petalPaint = new SKPaint
            {
                Color = SKColors.HotPink,
                IsAntialias = true
            };

            for (int i = 0; i < 8; i++)
            {
                double angle =
                    i * Math.PI * 2 / 8;

                float x =
                    centerX +
                    (float)Math.Cos(angle) * petalDistance;

                float y =
                    flowerY +
                    (float)Math.Sin(angle) * petalDistance;

                canvas.DrawCircle(
                    x,
                    y,
                    radius,
                    petalPaint);
            }

            using var centerPaint = new SKPaint
            {
                Color = SKColors.Gold,
                IsAntialias = true
            };

            canvas.DrawCircle(
                centerX,
                flowerY,
                radius,
                centerPaint);

            canvas.Restore();
        }
    }


}