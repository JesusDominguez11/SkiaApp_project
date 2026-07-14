using SkiaApp.Graphics;
using SkiaApp.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaApp
{
    public partial class MainPage : ContentPage
    {

        private readonly DrawingNavigationService _drawingService;
        private readonly IEnumerable<IDrawingRenderer> _renderers;
        private string _currentDrawing = "";

        public MainPage(
            DrawingNavigationService drawingService,
            IEnumerable<IDrawingRenderer> renderers)
        {
            InitializeComponent();

            _drawingService = drawingService;

            _renderers = renderers;

            _drawingService.ShowDrawingRequested += OnShowDrawing;
        }

        private void OnShowDrawing(string drawing)
        {
            _currentDrawing = drawing;


            var renderer = _renderers
                .First(x => x.Name == drawing);


            renderer.Updated += () =>
            {
                canvasView.InvalidateSurface();
            };


            renderer.StartAnimation();


            blazorWebView.IsVisible = false;
            canvasView.IsVisible = true;
        }

        private void OnPaintSurface(
            object sender,
            SKPaintSurfaceEventArgs e)
        {

            var renderer =
                _renderers.FirstOrDefault(
                    x => x.Name == _currentDrawing);

            if (renderer == null)
            {
                return;
            }

            renderer?.Draw(
                e.Surface.Canvas,
                e.Info.Width,
                e.Info.Height);
        }
    }
}
