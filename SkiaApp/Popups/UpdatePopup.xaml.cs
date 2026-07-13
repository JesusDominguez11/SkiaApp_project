using CommunityToolkit.Maui.Views;
using SkiaApp.Models;
using SkiaApp.Services;

namespace SkiaApp.Popups;

public partial class UpdatePopup : Popup
{
    private readonly UpdateInfo _updateInfo;
    private readonly IDownloadService _downloadService;
    private readonly IInstallerService _installerService;

    private bool _isDownloading;
    private double _progress;
    private string _status = "";
    public UpdatePopup(UpdateInfo updateInfo, IDownloadService downloadService, IInstallerService installerService)
	{
		InitializeComponent();

        _updateInfo = updateInfo;
        _downloadService = downloadService;
        _installerService = installerService;

        lblVersion.Text =
            $"Versión {_updateInfo.Version}";

        lblChanges.Text =
            _updateInfo.Changelog;
    }

    private void Close_Clicked(object sender, EventArgs e)
    {
        Close();
    }


    private async void Update_Clicked(object sender, EventArgs e)
    {
        _isDownloading = true;
        _status = "Descargando actualización...";
        OnPropertyChanged();

        var progress = new Progress<DownloadProgress>(p =>
        {
            lblChanges.Text =
                $"Descargando {p.Percentage:P0}";
        });


        var path = await _downloadService.DownloadFileAsync(
            _updateInfo.ApkUrl,
            progress);


        if (path != null)
        {
            _status = "Instalando...";
            OnPropertyChanged();

            var fileInfo = new FileInfo(path);

            System.Diagnostics.Debug.WriteLine($"Tamaño APK: {fileInfo.Length} bytes");

            await _installerService.InstallAsync(path);
        }
    }
    private void Close()
    {
        throw new NotImplementedException();
    }
}