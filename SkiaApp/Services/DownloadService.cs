using SkiaApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaApp.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly HttpClient _httpClient;
        public DownloadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string?> DownloadFileAsync(
        string url, IProgress<DownloadProgress>? progress = null)
        {
            try
            {
                using var response = await _httpClient.GetAsync(
                    url,
                    HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();

                long totalBytes =
                    response.Content.Headers.ContentLength ?? 0;

                var fileName = Path.GetFileName(new Uri(url).AbsolutePath);

                var filePath = Path.Combine(
                    FileSystem.CacheDirectory,
                    fileName);

                await using var input =
                    await response.Content.ReadAsStreamAsync();

                await using var output =
                    File.Create(filePath);

                var buffer = new byte[8192];

                long bytesReceived = 0;

                int bytesRead;

                while ((bytesRead = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await output.WriteAsync(buffer, 0, bytesRead);

                    bytesReceived += bytesRead;

                    progress?.Report(new DownloadProgress
                    {
                        BytesReceived = bytesReceived,
                        TotalBytes = totalBytes
                    });
                }

                return filePath;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
    }
}
