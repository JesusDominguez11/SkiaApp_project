
namespace SkiaApp.Services
{
    public class DrawingNavigationService
    {
        public event Action<string>? ShowDrawingRequested;

        public void ShowDrawing(string drawing)
        {
            ShowDrawingRequested?.Invoke(drawing);
        }
    }
}
