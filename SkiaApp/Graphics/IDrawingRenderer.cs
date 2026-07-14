using SkiaSharp;

namespace SkiaApp.Graphics
{
    public interface IDrawingRenderer
    {
        string Name { get; }

        event Action? Updated;

        void Draw(
            SKCanvas canvas,
            int width,
            int height);

        void StartAnimation();

        void StopAnimation();
    }
}
