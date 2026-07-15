
using Microsoft.Maui.Dispatching;
using SkiaSharp;
using System.Diagnostics;

namespace SkiaApp.Graphics;

public class FlowerRenderer : IDrawingRenderer
{
    private enum FlowerState
    {
        Growing,
        Wind
    }

    private FlowerState _state = FlowerState.Growing;

    public string Name => "flower";

    public event Action? Updated;

    private IDispatcherTimer? _timer;

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private double _lastTime;

    private float _growth;

    private float _windTime;

    // velocidad de crecimiento (10 segundos)
    private const float GrowthSpeed = 1f / 10f;

    // velocidad del viento
    private const float WindSpeed = 2f;

    // colores
    private readonly SKColor _skyTop = new(70, 170, 255);
    private readonly SKColor _skyBottom = new(220, 245, 255);

    private readonly SKColor _grass = new(46, 125, 50);
    private readonly SKColor _ground = new(90, 60, 35);

    private readonly SKColor _stem = new(30, 130, 40);

    private readonly SKColor _leaf = new(50, 170, 70);

    private readonly SKColor _petal = new(255, 70, 170);

    private readonly SKColor _flowerCenter = new(255, 210, 40);


    public void StartAnimation()
    {
        _growth = 0f;
        _windTime = 0f;

        _state = FlowerState.Growing;

        InitializeInsects();

        _lastTime = _stopwatch.Elapsed.TotalSeconds;

        _timer?.Stop();

        _timer = Dispatcher
            .GetForCurrentThread()
            .CreateTimer();

        _timer.Interval = TimeSpan.FromMilliseconds(16);

        _timer.Tick += (s, e) =>
        {
            double currentTime =
                _stopwatch.Elapsed.TotalSeconds;

            float deltaTime =
                (float)(currentTime - _lastTime);

            _lastTime = currentTime;

            switch (_state)
            {
                case FlowerState.Growing:

                    _growth +=
                        GrowthSpeed * deltaTime;

                    if (_growth >= 1f)
                    {
                        _growth = 1f;

                        _state = FlowerState.Wind;
                    }

                    break;

                case FlowerState.Wind:

                    _windTime +=
                        WindSpeed * deltaTime;

                    _windPulse =
                        MathF.Sin(_windTime * 0.7f);

                    _insectTime += 0.016f;

                    UpdateInsects();

                    break;
            }
            UpdateParticles(deltaTime);

            UpdateWindParticles();

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
        _canvasWidth = width;
        _canvasHeight = height;

        canvas.Clear(SKColors.Black);

        DrawBackground(canvas, width, height);

        DrawSun(canvas, width, height);

        DrawClouds(canvas, width, height);

        DrawMountains(canvas, width, height);

        DrawBackgroundTrees(canvas, width, height);

        DrawFog(canvas, width, height);

        DrawGround(canvas, width, height);

        DrawShadow(canvas, width, height);

        DrawStem(canvas, width, height);

        DrawLeaves(canvas, width, height);

        DrawFlower(canvas, width, height);

        DrawParticles(canvas, width, height);
        
        DrawWindParticles(canvas);

        DrawInsects(canvas);
    }


    private void DrawBackground(
    SKCanvas canvas,
    int width,
    int height)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,

            Shader = SKShader.CreateLinearGradient(
                new SKPoint(0, 0),
                new SKPoint(0, height),

                new[]
                {
                _skyTop,
                _skyBottom
                },

                null,

                SKShaderTileMode.Clamp)
        };

        canvas.DrawRect(
            0,
            0,
            width,
            height,
            paint);
    }

    private void DrawSun(
    SKCanvas canvas,
    int width,
    int height)
    {
        float x = width - 140;

        float y = 120;

        //----------------------------------
        // Glow
        //----------------------------------

        using (var glow = new SKPaint())
        {
            glow.IsAntialias = true;

            glow.Shader =
                SKShader.CreateRadialGradient(

                    new SKPoint(x, y),

                    90,

                    new[]
                    {
                    new SKColor(255,255,180,120),

                    SKColors.Transparent
                    },

                    null,

                    SKShaderTileMode.Clamp);

            canvas.DrawCircle(
                x,
                y,
                90,
                glow);
        }

        //----------------------------------
        // Sol
        //----------------------------------

        using var sun = new SKPaint
        {
            IsAntialias = true,

            Color = new SKColor(
                255,
                235,
                120)
        };

        canvas.DrawCircle(
            x,
            y,
            38,
            sun);
    }

    private void DrawGround(
    SKCanvas canvas,
    int width,
    int height)
    {
        float grassTop =
            height - 90;

        //--------------------------------
        // Tierra
        //--------------------------------

        using (var ground = new SKPaint())
        {
            ground.Color = _ground;

            canvas.DrawRect(
                0,
                grassTop,
                width,
                height,
                ground);
        }

        //--------------------------------
        // Césped
        //--------------------------------

        using var grass = new SKPaint
        {
            Color = _grass,

            StrokeWidth = 3,

            IsAntialias = true,

            StrokeCap = SKStrokeCap.Round
        };

        Random random = new Random(4);

        for (int x = 0; x < width; x += 4)
        {
            float bladeHeight =
                10 +
                (float)random.NextDouble() * 10;

            float wind =
                _state == FlowerState.Wind
                    ? MathF.Sin(
                        _windTime * 2f +
                        x * .05f) * 4f
                    : 0;

            canvas.DrawLine(

                x,

                grassTop,

                x + wind,

                grassTop - bladeHeight,

                grass);
        }
    }

    private void DrawShadow(
        SKCanvas canvas,
        int width,
        int height)
    {
        if (_growth <= 0)
            return;

        float centerX = width / 2f;

        float groundY = height - 90;

        float shadowWidth =
            45 +
            (_growth * 45);

        float shadowHeight =
            10 +
            (_growth * 6);

        float wind =
            _state == FlowerState.Wind
                ? MathF.Sin(_windTime) * 12f
                : 0;

        using var shadow = new SKPaint
        {
            IsAntialias = true,

            Color = new SKColor(
                0,
                0,
                0,
                55)
        };

        canvas.Save();

        canvas.Translate(
            wind * .4f,
            0);

        canvas.DrawOval(

            new SKRect(

                centerX - shadowWidth,

                groundY - shadowHeight,

                centerX + shadowWidth,

                groundY + shadowHeight),

            shadow);

        canvas.Restore();
    }

    private void DrawStem(
        SKCanvas canvas,
        int width,
        int height)
    {
        float centerX = width / 2f;

        float groundY = height - 90;

        float flowerY = height / 2f;

        //---------------------------------------
        // progreso del crecimiento
        //---------------------------------------

        float stemHeight =
            (groundY - flowerY) * _growth;

        float tipY =
            groundY - stemHeight;

        //---------------------------------------
        // viento
        //---------------------------------------

        float wind =
            _state == FlowerState.Wind
                ? MathF.Sin(_windTime) * 18f
                : 0f;

        //---------------------------------------
        // puntos de control
        //---------------------------------------

        SKPoint p0 =
            new(centerX, groundY);

        SKPoint p1 =
            new(centerX + wind * .20f,
                groundY - stemHeight * .30f);

        SKPoint p2 =
            new(centerX + wind * .80f,
                groundY - stemHeight * .70f);

        SKPoint p3 =
            new(centerX + wind,
                tipY);

        //---------------------------------------
        // dibujar el tallo por segmentos
        //---------------------------------------

        using var paint = new SKPaint
        {
            IsAntialias = true,
            Color = _stem,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 9,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        const int segments = 70;

        SKPoint previous = p0;

        for (int i = 1; i <= segments * _growth; i++)
        {
            float t = i / (float)segments;

            float u = 1 - t;

            SKPoint point = new(
                u * u * u * p0.X +
                3 * u * u * t * p1.X +
                3 * u * t * t * p2.X +
                t * t * t * p3.X,

                u * u * u * p0.Y +
                3 * u * u * t * p1.Y +
                3 * u * t * t * p2.Y +
                t * t * t * p3.Y);

            canvas.DrawLine(previous, point, paint);

            previous = point;
        }
    }

    private void DrawLeaves(
        SKCanvas canvas,
        int width,
        int height)
    {
        if (_growth < 0.35f)
            return;

        float centerX = width / 2f;

        float groundY = height - 90;

        float flowerY = height / 2f;

        //---------------------------------------
        // progreso de aparición
        //---------------------------------------

        float leafGrowth =
            (_growth - 0.35f) / 0.65f;

        leafGrowth =
            Math.Clamp(
                leafGrowth,
                0,
                1);

        //---------------------------------------
        // viento
        //---------------------------------------

        float wind =
            _state == FlowerState.Wind
                ? _windPulse * 8f
                : 0f;

        //---------------------------------------
        // posiciones
        //---------------------------------------

        float leaf1Y =
            groundY - (groundY - flowerY) * .38f;

        float leaf2Y =
            groundY - (groundY - flowerY) * .62f;

        DrawLeaf(
            canvas,

            new SKPoint(centerX, leaf1Y),

            -1,

            leafGrowth,

            wind);

        DrawLeaf(
            canvas,

            new SKPoint(centerX, leaf2Y),

            1,

            leafGrowth,

            wind);
    }

    private void DrawLeaf(
    SKCanvas canvas,

    SKPoint origin,

    int direction,

    float growth,

    float wind)
    {
        float length =
            60 * growth;

        float width =
            22 * growth;

        float tipX =
            origin.X +
            direction * length +
            wind;

        float tipY =
            origin.Y - 12;

        var path =
            new SKPath();

        //---------------------------------------
        // mitad superior
        //---------------------------------------

        path.MoveTo(origin);

        path.CubicTo(

            origin.X +
            direction * 12,

            origin.Y - width,

            tipX -
            direction * 18,

            tipY - width,

            tipX,

            tipY);

        //---------------------------------------
        // mitad inferior
        //---------------------------------------

        path.CubicTo(

            tipX -
            direction * 18,

            tipY + width,

            origin.X +
            direction * 12,

            origin.Y + width,

            origin.X,

            origin.Y);

        //---------------------------------------

        using var fill =
            new SKPaint
            {
                IsAntialias = true,

                Color = _leaf,

                Style = SKPaintStyle.Fill
            };

        canvas.DrawPath(
            path,
            fill);

        //---------------------------------------
        // nervadura
        //---------------------------------------

        using var vein =
            new SKPaint
            {
                IsAntialias = true,

                Color = new SKColor(
                    210,
                    255,
                    210),

                StrokeWidth = 2,

                Style = SKPaintStyle.Stroke
            };

        canvas.DrawLine(
            origin,

            new SKPoint(
                tipX,
                tipY),

            vein);
    }

private readonly SKColor _centerDark =
    new(150, 90, 10);
    private void DrawFlower(
        SKCanvas canvas,
        int width,
        int height)
    {
        if (_growth < 0.70f)
            return;

        float centerX = width / 2f;

        float flowerY = height / 2f;

        _flowerCenterX = centerX;
        _flowerCenterY = flowerY;

        //----------------------------------------
        // progreso de aparición
        //----------------------------------------

        float bloom =
            (_growth - .70f) / .30f;

        bloom = Math.Clamp(
            bloom,
            0,
            1);

        //----------------------------------------
        // viento
        //----------------------------------------

        float wind =
    _state == FlowerState.Wind
        ? _windPulse * 5f
        : 0f;

        float radius =
            38 * bloom;

        float petalDistance =
            radius * 1.45f;

        canvas.Save();

        canvas.Translate(
            wind,
            0);

        //----------------------------------------
        // pétalos
        //----------------------------------------

        const int petals = 8;

        for (int i = 0; i < petals; i++)
        {
            float angle =
                360f / petals * i;

            DrawPetal(
                canvas,
                centerX,
                flowerY,
                radius,
                petalDistance,
                angle,
                i);
        }

        //----------------------------------------
        // centro
        //----------------------------------------

        DrawFlowerCenter(
            canvas,
            centerX,
            flowerY,
            radius);

        canvas.Restore();
    }

    private void DrawFlowerCenter(
        SKCanvas canvas,
        float x,
        float y,
        float radius)
    {

        //--------------------------------
        // sombra exterior
        //--------------------------------

        using var shadow =
            new SKPaint
            {
                IsAntialias = true,

                Color =
                    new SKColor(
                        80,
                        40,
                        0,
                        80)
            };


        canvas.DrawCircle(
            x + 3,
            y + 4,
            radius * .65f,
            shadow);



        //--------------------------------
        // centro
        //--------------------------------

        using var center =
            new SKPaint
            {
                IsAntialias = true,

                Shader =
                    SKShader.CreateRadialGradient(

                        new SKPoint(
                            x - radius * .2f,
                            y - radius * .2f),

                        radius * .8f,

                        new[]
                        {
                        SKColors.Yellow,
                        _centerDark
                        },

                        null,

                        SKShaderTileMode.Clamp)
            };


        canvas.DrawCircle(
            x,
            y,
            radius * .6f,
            center);



        //--------------------------------
        // semillas
        //--------------------------------

        using var seed =
            new SKPaint
            {
                IsAntialias = true,

                Color =
                    new SKColor(
                        90,
                        50,
                        0)
            };


        Random random =
            new(5);


        for (int i = 0; i < 60; i++)
        {
            double a =
                random.NextDouble()
                * Math.PI * 2;


            float r =
                (float)random.NextDouble()
                * radius * .45f;


            canvas.DrawCircle(

                x +
                MathF.Cos((float)a)
                * r,

                y +
                MathF.Sin((float)a)
                * r,

                1.5f,

                seed);
        }
    }

    private void DrawPetal(
    SKCanvas canvas,

    float centerX,

    float centerY,

    float radius,

    float distance,

    float angle,

    int index)
    {
        canvas.Save();

        float flutter =
            MathF.Sin(
                _windTime * 2f +
                index) * 3f;

        canvas.Translate(centerX, centerY);

        canvas.RotateDegrees(
            angle + flutter);

        using var paint =
            new SKPaint
            {
                IsAntialias = true,

                Shader =
                    SKShader.CreateLinearGradient(

                        new SKPoint(0, -radius),

                        new SKPoint(0, radius),

                        new[]
                        {
                        SKColors.White,

                        _petal
                        },

                        null,

                        SKShaderTileMode.Clamp)
            };

        canvas.DrawOval(

            new SKRect(

                -radius * .45f,

                -distance - radius,

                radius * .45f,

                -distance + radius),

            paint);

        canvas.Restore();
    }

    private void DrawParticles(
        SKCanvas canvas,
        int width,
        int height)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true
        };

        foreach (var p in _particles)
        {
            float alpha =
                1f -
                (p.Life / p.MaxLife);

            paint.Color =
                new SKColor(
                    255,
                    240,
                    120,
                    (byte)(alpha * 180));

            canvas.DrawCircle(
                p.Position,
                p.Size,
                paint);
        }
    }

    private float _flowerCenterX;
    private float _flowerCenterY;
    private void UpdateParticles(float deltaTime)
    {
        Random random = new Random();

        // Crear nuevas partículas
        if (_state == FlowerState.Wind)
        {
            while (_particles.Count < 35)
            {

                float angle =
    (float)(random.NextDouble() * Math.PI * 2);

                float distance =
                    (float)random.NextDouble() * 15;

                _particles.Add(new Particle
                {
                    Position = new SKPoint(
                    _flowerCenterX + MathF.Cos(angle) * distance,
                    _flowerCenterY + MathF.Sin(angle) * distance),

                    Velocity = new SKPoint(
    10 + (float)random.NextDouble() * 15,
    -15 + (float)random.NextDouble() * 30),

                    Size = 2 + (float)random.NextDouble() * 2,

                    Life = 0,

                    MaxLife = 6 + (float)random.NextDouble() * 3
                });
            }
        }

        //------------------------------------

        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            var p = _particles[i];

            float wind =
                MathF.Sin(_windTime + i * .4f);

            p.Position.X +=
                (p.Velocity.X + wind * 12f)
                * deltaTime;

            p.Position.Y +=
                p.Velocity.Y
                * deltaTime;

            p.Life += deltaTime;

            if (p.Life >= p.MaxLife)
            {
                _particles.RemoveAt(i);
                continue;
            }

            _particles[i] = p;
        }


    }

    private readonly List<Particle> _particles = [];

    private class Particle
    {
        public SKPoint Position;

        public SKPoint Velocity;

        public float Size;

        public float Life;

        public float MaxLife;
    }



    private readonly SKColor _mountain =
    new(110, 140, 160);
    private void DrawMountains(
    SKCanvas canvas,
    int width,
    int height)
    {
        float baseY = height - 90;

        DrawMountainLayer(
            canvas,
            width,
            baseY,
            170,
            new SKColor(130, 150, 170));

        DrawMountainLayer(
            canvas,
            width,
            baseY + 30,
            120,
            new SKColor(90, 120, 145));

        DrawMountainLayer(
            canvas,
            width,
            baseY + 60,
            80,
            new SKColor(60, 90, 110));
    }

    private void DrawMountainLayer(
    SKCanvas canvas,
    int width,
    float baseY,
    float heightOffset,
    SKColor color)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Color = color,
            Style = SKPaintStyle.Fill
        };


        var path = new SKPath();

        path.MoveTo(0, baseY);


        int points = 14;

        float step =
            width / (float)points;


        Random random =
            new Random((int)heightOffset);


        for (int i = 0; i <= points; i++)
        {
            float x =
                i * step;


            float variation =
                (float)random.NextDouble()
                * heightOffset
                * .6f;


            float y =
                baseY -
                variation;


            if (i == 0)
            {
                path.MoveTo(x, y);
            }
            else
            {
                float previousX =
                    (i - 1) * step;

                float controlX =
                    previousX + step / 2;


                path.QuadTo(
                    controlX,
                    y - 40,
                    x,
                    y);
            }
        }


        path.LineTo(
            width,
            baseY + 100);


        path.LineTo(
            0,
            baseY + 100);


        path.Close();


        canvas.DrawPath(
            path,
            paint);
    }


    private void DrawCloud(
    SKCanvas canvas,
    float x,
    float y,
    float scale)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Color = new SKColor(
                255,
                255,
                255,
                190)
        };


        var path = new SKPath();


        path.MoveTo(
            x,
            y);


        path.CubicTo(
            x - 40 * scale,
            y - 30 * scale,
            x + 20 * scale,
            y - 70 * scale,
            x + 50 * scale,
            y - 35 * scale);


        path.CubicTo(
            x + 100 * scale,
            y - 80 * scale,
            x + 150 * scale,
            y - 20 * scale,
            x + 130 * scale,
            y + 10 * scale);


        path.CubicTo(
            x + 80 * scale,
            y + 40 * scale,
            x + 20 * scale,
            y + 30 * scale,
            x,
            y);


        path.Close();


        canvas.DrawPath(
            path,
            paint);
    }

    private void DrawClouds(
        SKCanvas canvas,
        int width,
        int height)
    {
        float movement =
            (_windTime * 8) %
            (width + 300);


        DrawCloud(
            canvas,
            movement - 200,
            120,
            1.2f);


        DrawCloud(
            canvas,
            width - movement,
            220,
            .8f);


        DrawCloud(
            canvas,
            width * .4f,
            90,
            .6f);
    }

    private readonly SKColor _treesBack =
    new(45, 90, 65);
    private readonly SKColor _forestBack =
    new(35, 75, 55);
    private void DrawBackgroundTrees(
        SKCanvas canvas,
        int width,
        int height)
    {
        float ground =
            height - 90;


        Random random =
            new Random(20);


        for (int i = 0; i < 14; i++)
        {
            float x =
                i * (width / 13f)
                + random.Next(-20, 20);


            float treeHeight =
                random.Next(90, 160);


            float treeWidth =
                treeHeight * .55f;


            DrawPineTree(
                canvas,
                x,
                ground,
                treeWidth,
                treeHeight);
        }
    }
    private void DrawPineTree(
    SKCanvas canvas,
    float x,
    float ground,
    float width,
    float height)
    {
        //--------------------------------
        // tronco
        //--------------------------------

        using var trunk =
            new SKPaint
            {
                Color =
                    new SKColor(
                        80,
                        55,
                        40),

                IsAntialias = true
            };


        canvas.DrawRect(
            x - width * .04f,
            ground - height * .25f,
            x + width * .04f,
            ground,
            trunk);


        //--------------------------------
        // copa por capas
        //--------------------------------

        using var leaves =
            new SKPaint
            {
                Color = _forestBack,
                IsAntialias = true
            };


        float top =
            ground - height;


        DrawTreeLayer(
            canvas,
            x,
            top + height * .20f,
            width * .25f,
            height * .35f,
            leaves);


        DrawTreeLayer(
            canvas,
            x,
            top + height * .42f,
            width * .38f,
            height * .38f,
            leaves);


        DrawTreeLayer(
            canvas,
            x,
            top + height * .68f,
            width * .50f,
            height * .35f,
            leaves);
    }

    private void DrawTreeLayer(
        SKCanvas canvas,
        float x,
        float y,
        float width,
        float height,
        SKPaint paint)
    {
        var path =
            new SKPath();


        path.MoveTo(
            x,
            y - height);


        path.LineTo(
            x - width,
            y + height);


        path.LineTo(
            x + width,
            y + height);


        path.Close();


        canvas.DrawPath(
            path,
            paint);
    }

    private readonly SKColor _fogColor =
    new(220, 235, 240, 80);
    private void DrawFog(
    SKCanvas canvas,
    int width,
    int height)
    {
        using var paint =
            new SKPaint
            {
                IsAntialias = true,
                Color = _fogColor
            };


        float horizon =
            height * .55f;


        var gradient =
            SKShader.CreateLinearGradient(
                new SKPoint(
                    0,
                    horizon),

                new SKPoint(
                    0,
                    height),

                new[]
                {
                new SKColor(
                    220,
                    235,
                    240,
                    0),

                new SKColor(
                    220,
                    235,
                    240,
                    100)
                },

                null,

                SKShaderTileMode.Clamp);



        paint.Shader = gradient;


        canvas.DrawRect(
            0,
            horizon,
            width,
            height,
            paint);
    }

    private readonly SKColor _windColor =
    new(
        255,
        255,
        255,
        35);
    private float _windPulse;
    private readonly SKColor _airFlow =
        new(
            255,
            255,
            255,
            25);
    private class WindParticle
    {
        public SKPoint Position;

        public float Offset;

        public float Life;
    }
    private readonly List<WindParticle> _windParticles = [];
    private void UpdateWindParticles()
    {
        if (_state != FlowerState.Wind)
            return;


        Random random = new();


        while (_windParticles.Count < 18)
        {
            _windParticles.Add(new WindParticle
            {
                Position =
                    new SKPoint(
                        _flowerCenterX - 80,
                        _flowerCenterY +
                        random.Next(-80, 80)),

                Offset =
                    (float)random.NextDouble()
                    * MathF.PI * 2,

                Life = 0
            });
        }


        for (int i = _windParticles.Count - 1; i >= 0; i--)
        {
            var p =
                _windParticles[i];


            p.Position =
                new SKPoint(
                    p.Position.X + 0.8f,

                    p.Position.Y +
                    MathF.Sin(
                        _windTime +
                        p.Offset)
                    * 0.5f);


            p.Life += .01f;


            if (p.Life > 1)
                _windParticles.RemoveAt(i);
        }
    }

    private void DrawWindParticles(
    SKCanvas canvas)
    {
        using var paint =
            new SKPaint
            {
                IsAntialias = true,

                Color = _airFlow,

                StrokeWidth = 8,

                Style =
                    SKPaintStyle.Stroke
            };


        foreach (var p in _windParticles)
        {
            var path =
                new SKPath();


            path.MoveTo(
                p.Position);


            path.CubicTo(

                p.Position.X + 5,
                p.Position.Y - 8,

                p.Position.X + 30,
                p.Position.Y + 8,

                p.Position.X + 55,
                p.Position.Y);


            canvas.DrawPath(
                path,
                paint);
        }
    }

    private int _canvasWidth;
    private int _canvasHeight;


    private class Butterfly
    {
        public SKPoint Position;

        public float Phase;

        public float Speed;

        public float Scale;

        public float Height;

        public SKColor Color;
    }


    private class Bee
    {
        public SKPoint Position;

        public float Phase;

        public float Speed;

        public float Radius;
    }

    private readonly List<Butterfly> _butterflies = [];

    private readonly List<Bee> _bees = [];

    private float _insectTime;

    private void InitializeInsects()
    {
        if (_butterflies.Count > 0)
            return;


        _butterflies.Add(
            new Butterfly
            {
                Position = new SKPoint(0, 0),
                Phase = 0,
                Speed = .8f,
                Scale = .8f,
                Height = 80,
                Color = new SKColor(170, 120, 255)
            });


        _butterflies.Add(
            new Butterfly
            {
                Position = new SKPoint(0, 0),
                Phase = 3,
                Speed = .6f,
                Scale = .6f,
                Height = 130,
                Color = new SKColor(255, 150, 200)
            });



        _bees.Add(
            new Bee
            {
                Phase = 0,
                Speed = 1.8f,
                Radius = 70
            });
    }

    private void UpdateInsects()
    {
        foreach (var butterfly in _butterflies)
        {
            float t =
                _insectTime * butterfly.Speed +
                butterfly.Phase;


            butterfly.Position =
                new SKPoint(

                    250 +
                    MathF.Sin(t) * 120,

                    180 +
                    MathF.Sin(t * 2)
                    * 30
                    +
                    MathF.Sin(t)
                    * 20);
        }



        foreach (var bee in _bees)
        {
            float t =
                _insectTime * bee.Speed +
                bee.Phase;


            float flowerX =
                _canvasWidth / 2f;


            float flowerY =
                _canvasHeight / 2f;


            bee.Position =
                new SKPoint(

                    flowerX +
                    MathF.Cos(t)
                    * bee.Radius,


                    flowerY +
                    MathF.Sin(t * 1.7f)
                    * 30);
        }
    }

    private void DrawInsects(
    SKCanvas canvas)
    {
        foreach (var butterfly in _butterflies)
        {
            DrawButterfly(
                canvas,
                butterfly);
        }


        foreach (var bee in _bees)
        {
            DrawBee(
                canvas,
                bee);
        }
    }

    private void DrawButterfly(
    SKCanvas canvas,
    Butterfly butterfly)
    {
        canvas.Save();


        canvas.Translate(
            butterfly.Position);


        float flap =
            MathF.Sin(
                _insectTime * 8 +
                butterfly.Phase);



        float wing =
            18 +
            flap * 8;



        using var wingPaint =
            new SKPaint
            {
                IsAntialias = true,
                Color = butterfly.Color
            };


        using var bodyPaint =
            new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.DarkGray
            };



        // ala izquierda

        var left =
            new SKPath();


        left.MoveTo(0, 0);


        left.CubicTo(
            -wing,
            -25,
            -45,
            -5,
            -25,
            25);


        left.CubicTo(
            -10,
            35,
            0,
            15,
            0,
            0);



        canvas.DrawPath(
            left,
            wingPaint);



        // ala derecha

        var right =
            new SKPath();


        right.MoveTo(0, 0);


        right.CubicTo(
            wing,
            -25,
            45,
            -5,
            25,
            25);


        right.CubicTo(
            10,
            35,
            0,
            15,
            0,
            0);



        canvas.DrawPath(
            right,
            wingPaint);



        // cuerpo

        canvas.DrawOval(

            new SKRect(
                -3,
                -12,
                3,
                12),

            bodyPaint);



        // antenas

        using var antenna =
            new SKPaint
            {
                Color = SKColors.Black,
                StrokeWidth = 1,
                IsAntialias = true
            };


        canvas.DrawLine(
            0,
            -10,
            -8,
            -18,
            antenna);


        canvas.DrawLine(
            0,
            -10,
            8,
            -18,
            antenna);



        canvas.Restore();
    }

    private void DrawBee(
    SKCanvas canvas,
    Bee bee)
    {
        canvas.Save();

        canvas.Translate(
            bee.Position);


        float wingMove =
            MathF.Sin(
                _insectTime * 18)
            * 5;



        //--------------------------------
        // sombra
        //--------------------------------

        using var shadow =
            new SKPaint
            {
                IsAntialias = true,

                Color =
                    new SKColor(
                        0,
                        0,
                        0,
                        40)
            };


        canvas.DrawOval(
            new SKRect(
                -10,
                8,
                10,
                14),
            shadow);



        //--------------------------------
        // alas
        //--------------------------------

        using var wingPaint =
            new SKPaint
            {
                IsAntialias = true,

                Color =
                    new SKColor(
                        220,
                        240,
                        255,
                        150)
            };


        canvas.Save();


        canvas.RotateDegrees(
            wingMove);


        canvas.DrawOval(
            new SKRect(
                -12,
                -12,
                0,
                5),
            wingPaint);


        canvas.Restore();



        canvas.Save();

        canvas.RotateDegrees(
            -wingMove);


        canvas.DrawOval(
            new SKRect(
                0,
                -12,
                12,
                5),
            wingPaint);


        canvas.Restore();



        //--------------------------------
        // cuerpo
        //--------------------------------


        using var body =
            new SKPaint
            {
                IsAntialias = true,

                Shader =
                    SKShader.CreateLinearGradient(

                        new SKPoint(-10, 0),

                        new SKPoint(10, 0),

                        new[]
                        {
                        SKColors.Gold,
                        SKColors.Orange
                        },

                        null,

                        SKShaderTileMode.Clamp)
            };


        canvas.DrawOval(

            new SKRect(
                -10,
                -5,
                10,
                5),

            body);



        //--------------------------------
        // rayas
        //--------------------------------

        using var stripes =
            new SKPaint
            {
                Color = SKColors.Black,

                StrokeWidth = 2,

                IsAntialias = true
            };


        canvas.DrawLine(
            -3,
            -5,
            -3,
            5,
            stripes);


        canvas.DrawLine(
            3,
            -5,
            3,
            5,
            stripes);



        //--------------------------------
        // cabeza
        //--------------------------------

        using var head =
            new SKPaint
            {
                Color = SKColors.Black,

                IsAntialias = true
            };


        canvas.DrawCircle(
            11,
            0,
            4,
            head);



        canvas.Restore();
    }
}

