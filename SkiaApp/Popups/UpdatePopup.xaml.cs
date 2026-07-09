using CommunityToolkit.Maui.Views;
using SkiaApp.Models;

namespace SkiaApp.Popups;

public partial class UpdatePopup : Popup
{
    private readonly UpdateInfo _updateInfo;

    public UpdatePopup(UpdateInfo updateInfo)
	{
		InitializeComponent();

        _updateInfo = updateInfo;

        lblVersion.Text =
            $"Versión {_updateInfo.Version}";

        lblChanges.Text =
            _updateInfo.Changelog;
    }

    private void Close_Clicked(object sender, EventArgs e)
    {
        Close();
    }


    private void Update_Clicked(object sender, EventArgs e)
    {
        // después aquí llamaremos DownloadService
        Close();
    }
    private void Close()
    {
        throw new NotImplementedException();
    }
}