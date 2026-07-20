using Microsoft.Maui.Storage;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SkiaApp.Graphics
{
    internal class SvgRenderer : IDrawingRenderer
    {
        public string Name => _fileName;

        public event Action? Updated;


        private readonly string _fileName;

        private readonly List<SvgPathData> _paths = [];

        private IDispatcherTimer? _timer;


        public SvgRenderer(string fileName)
        {
            _fileName = fileName;

            LoadSvg();
        }


        private void LoadSvg()
        {
            _paths.Clear();

            using var stream = FileSystem
                .OpenAppPackageFileAsync($"Drawings/{_fileName}.svg")
                .Result;


            var document = XDocument.Load(stream);


            XNamespace ns = "http://www.w3.org/2000/svg";


            foreach (var element in document.Descendants(ns + "path"))
            {
                var d = element.Attribute("d")?.Value;


                if (string.IsNullOrWhiteSpace(d))
                    continue;


                var path = SKPath.ParseSvgPathData(d);


                if (path == null)
                    continue;


                // Aplicar transformaciones de los padres <g>
                var transform = GetParentTransform(element);

                path.Transform(transform);


                var color = GetFillColor(element);


                using var measure = new SKPathMeasure(path);


                _paths.Add(new SvgPathData
                {
                    Path = path,
                    Fill = color,
                    Length = measure.Length,
                    Progress = 1,
                    Finished = true
                });
            }


            Debug.WriteLine(
                $"Paths encontrados: {_paths.Count}");


            if (_paths.Count > 0)
            {
                Debug.WriteLine(
                    $"Primer path: {_paths[0].Path.Bounds}");
            }
        }



        private SKColor GetFillColor(XElement element)
        {
            var style =
                element.Attribute("style")?.Value;


            if (!string.IsNullOrWhiteSpace(style))
            {
                foreach (var item in style.Split(';'))
                {
                    if (!item.StartsWith("fill:"))
                        continue;


                    var value = item
                        .Substring(5)
                        .Trim();


                    if (value.StartsWith("#"))
                    {
                        return SKColor.Parse(value);
                    }
                }
            }


            var fill =
                element.Attribute("fill")?.Value;


            if (!string.IsNullOrWhiteSpace(fill)
                && fill.StartsWith("#"))
            {
                return SKColor.Parse(fill);
            }


            return SKColors.White;
        }



        private SKMatrix GetParentTransform(XElement element)
        {
            var matrix =
                SKMatrix.CreateIdentity();


            var parents =
                element
                .Ancestors()
                .Reverse();



            foreach (var parent in parents)
            {
                var transform =
                    parent.Attribute("transform")?.Value;


                if (string.IsNullOrWhiteSpace(transform))
                    continue;



                if (transform.StartsWith("scale"))
                {
                    var values =
                        GetNumbers(transform);


                    float sx = values[0];

                    float sy =
                        values.Count > 1
                        ? values[1]
                        : sx;


                    matrix =
                        SKMatrix.Concat(
                            matrix,
                            SKMatrix.CreateScale(
                                sx,
                                sy));
                }



                else if (transform.StartsWith("matrix"))
                {
                    var values =
                        GetNumbers(transform);


                    var m = new SKMatrix
                    {
                        ScaleX = values[0],
                        SkewY = values[1],
                        SkewX = values[2],
                        ScaleY = values[3],
                        TransX = values[4],
                        TransY = values[5]
                    };


                    matrix =
                        SKMatrix.Concat(
                            matrix,
                            m);
                }
            }


            return matrix;
        }



        private List<float> GetNumbers(string value)
        {
            return value
                .Replace("matrix(", "")
                .Replace("scale(", "")
                .Replace(")", "")
                .Split(
                    new[] { ',', ' ' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                    float.Parse(
                        x,
                        CultureInfo.InvariantCulture))
                .ToList();
        }




        public void Draw(
            SKCanvas canvas,
            int width,
            int height)
        {
            canvas.Clear(SKColors.Black);


            if (_paths.Count == 0)
                return;


            var bounds = _paths[0].Path.Bounds;


            Debug.WriteLine(
                $"DRAW BOUNDS: {bounds}");


            canvas.Save();


            // centrar usando el primer path como prueba
            canvas.Translate(
                width / 2f - bounds.MidX,
                height / 2f - bounds.MidY);



            using var paint = new SKPaint
            {
                Color = SKColors.Red,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };


            canvas.DrawPath(
                _paths[0].Path,
                paint);



            canvas.Restore();
        }



        public void StartAnimation()
        {
            Updated?.Invoke();
        }



        public void StopAnimation()
        {
            _timer?.Stop();
        }
    }
}