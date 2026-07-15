using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using SkiaApp.Models;
using SkiaApp.Popups;
using SkiaApp.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace SkiaApp;

public partial class DashboardPage : ContentPage
{
    private double _animationProgress = 0.0;

    public DashboardPage(IVersionService versionService, IUpdateService updateService, IDownloadService downloadService, IInstallerService installerService)
	{
		InitializeComponent();

        _versionService = versionService;
        _updateService = updateService;
        _downloadService = downloadService;
        _installerService = installerService;
        Title = $"Dashboard v{_versionService.CurrentVersion}";

        lblVersion.Text = $"Versión {_versionService.CurrentVersion} (Build {_versionService.Build})";
    }

    private bool _updateDismissed;
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_updateDismissed)
            return;

        var update = await _updateService.CheckForUpdatesAsync();

        if (update != null)
        {
            this.ShowPopup(
                new UpdatePopup(
                    update, 
                    _downloadService, 
                    _installerService,
                    () => _updateDismissed = true),
                new PopupOptions
                {
                    CanBeDismissedByTappingOutsideOfPopup = false,
                    PageOverlayColor = Color.FromArgb("#80000080")
                });
        }
    }

    private readonly IVersionService _versionService;
    private readonly IUpdateService _updateService;
    private readonly IDownloadService _downloadService;
    private readonly IInstallerService _installerService;

}