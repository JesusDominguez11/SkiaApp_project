using CommunityToolkit.Maui.Views;
using SkiaApp.Models;
using SkiaApp.Services;
using System.Drawing;

namespace SkiaApp.Popups;

public partial class UpdatePopup : Popup
{
    private readonly UpdateInfo _updateInfo;
    private readonly IDownloadService _downloadService;
    private readonly IInstallerService _installerService;
    private readonly Action _onDismissed;

    private bool _isDownloading;
    private double _progress;
    public UpdatePopup(
        UpdateInfo updateInfo, 
        IDownloadService downloadService, 
        IInstallerService installerService,
        Action onDismissed)
	{
		InitializeComponent();

        _updateInfo = updateInfo;
        _downloadService = downloadService;
        _installerService = installerService;
        _onDismissed = onDismissed;

        lblVersion.Text =
            $"Versión {_updateInfo.Version}";
    }

    private bool _isUpdating = false;
    private async void Update_Clicked(object sender, EventArgs e)
    {
        _isUpdating = true;

        btnUpdate.IsEnabled = false;
        btnUpdate.BackgroundColor = Colors.Black;
        btnLater.IsEnabled = false;
        btnLater.BackgroundColor = Colors.Black;

        lblStatus.Text = "Descargando actualización...";
        OnPropertyChanged();

        var progress = new Progress<DownloadProgress>(p =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                progressBar.Progress = p.Percentage;

                lblStatus.Text = $"Descargando {p.Percentage:P0}";
            });
        });

        var path = await _downloadService.DownloadFileAsync(
            _updateInfo.ApkUrl,
            progress);


        if (path != null)
        {
            lblStatus.Text = "Instalando actualización...";
            OnPropertyChanged();

            await _installerService.InstallAsync(path);
        }
    }

    private async void Close_Clicked(object sender, EventArgs e)
    {
        if (_isUpdating)
            return;

        _onDismissed?.Invoke();

        await base.CloseAsync();
    }
}