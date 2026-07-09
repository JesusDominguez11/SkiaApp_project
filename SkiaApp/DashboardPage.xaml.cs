using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaApp;

public partial class DashboardPage : ContentPage
{
    private double _animationProgress = 0.0;

    public DashboardPage()
	{
		InitializeComponent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        IniciarAnimacionFlor();
    }

    private void IniciarAnimacionFlor()
    {
        // Creamos una animación de 0 a 1 que dura 1.5 segundos (1500 ms)
        var flowerAnimation = new Animation(v =>
        {
            _animationProgress = v;

            // Forzamos a SkiaSharp a volver a ejecutar OnPaintSurface
            canvasView.InvalidateSurface();
        },
        start: 0.0,
        end: 1.0,
        easing: Easing.CubicOut); // Efecto suave al terminar

        // Ejecutamos la animación en la página actual
        flowerAnimation.Commit(this, "AnimacionFlor", length: 10000, repeat: () => false);
    }

    private float _progress = 0;
    private const float AnimationSpeed = 0.01f;
    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        int width = e.Info.Width;
        int height = e.Info.Height;

        float centerXP = width / 2f;
        float centerYP = height / 2f;

        // El tamaño máximo final de la flor dependiente del tamaño de la pantalla
        float maxRadius = Math.Min(width, height) / 8f;

        canvas.Clear(SKColors.White);

        // Multiplicamos el radio por el progreso actual de la animación
        float currentRadius = maxRadius * (float)_animationProgress;

        // Si la animación no ha empezado, no dibujamos nada
        if (currentRadius <= 0) return;

        // 1. Dibujar Pétalos Animados
        using (var petalPaint = new SKPaint { IsAntialias = true, Color = SKColors.HotPink, Style = SKPaintStyle.Fill })
        {
            int numPetals = 8;

            // La distancia del pétalo al centro también escala con la animación
            float petalDistance = currentRadius * 1.5f;

            for (int i = 0; i < numPetals; i++)
            {
                double angle = (i * 2 * Math.PI) / numPetals;
                float petalCenterX = centerXP + (float)(Math.Cos(angle) * petalDistance);
                float petalCenterY = centerYP + (float)(Math.Sin(angle) * petalDistance);

                canvas.DrawCircle(petalCenterX, petalCenterY, currentRadius, petalPaint);
            }
        }

        // 2. Dibujar Centro Animado
        using (var centerPaint = new SKPaint { IsAntialias = true, Color = SKColors.Gold, Style = SKPaintStyle.Fill })
        {
            canvas.DrawCircle(centerXP, centerYP, currentRadius, centerPaint);
        }
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