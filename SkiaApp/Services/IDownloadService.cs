using SkiaApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaApp.Services
{
    public interface IDownloadService
    {
        Task<string?> DownloadApkAsync(
            string url,
            IProgress<DownloadProgress>? progress = null);
    }
}
