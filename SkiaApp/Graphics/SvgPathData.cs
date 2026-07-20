using SkiaSharp;

namespace SkiaApp.Graphics
{
    public class SvgPathData
    {
        public SKPath Path { get; set; } = new();

        public SKColor Fill { get; set; }

        public float Length { get; set; }

        public float Progress { get; set; }

        public bool Finished { get; set; }
    }
}
