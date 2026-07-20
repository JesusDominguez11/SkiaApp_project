using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using SkiaApp.Graphics;
using Plugin.Maui.Audio;

#if ANDROID
using SkiaApp.Platforms.Android;
#endif
using SkiaApp.Services;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace SkiaApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .AddAudio()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMudServices();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<IVersionService, VersionService>();
            builder.Services.AddSingleton<IUpdateService, UpdateService>();
            builder.Services.AddSingleton<IDownloadService, DownloadService>();
            #if ANDROID
            IServiceCollection serviceCollection = builder.Services.AddSingleton<IInstallerService, AndroidInstallerService>();
            #endif
            builder.Services.AddSingleton<DrawingNavigationService>();
            builder.Services.AddSingleton<IDrawingRenderer, FlowerRenderer>();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<BackgroundMusicService>();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
