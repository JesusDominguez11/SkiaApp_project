using Android.Content;
using SkiaApp.Services;

namespace SkiaApp.Platforms.Android
{
    public class AndroidInstallerService : IInstallerService
    {
        public Task InstallAsync(string apkPath)
        {
            try
            {
                var context = global::Android.App.Application.Context;

                var file = new Java.IO.File(apkPath);

                System.Diagnostics.Debug.WriteLine($"APK existe: {file.Exists()}");

                var uri = AndroidX.Core.Content.FileProvider.GetUriForFile(
                    context,
                    $"{context.PackageName}.fileprovider",
                    file);

                System.Diagnostics.Debug.WriteLine(
           $"URI: {uri}");

                var intent = new Intent(Intent.ActionView);

                intent.SetDataAndType(
                    uri,
                    "application/vnd.android.package-archive");

                intent.AddFlags(
                    ActivityFlags.GrantReadUriPermission |
                    ActivityFlags.NewTask);

                System.Diagnostics.Debug.WriteLine(
        "Lanzando instalador");

                context.StartActivity(intent);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
