using SkiaApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkiaApp.Services
{
    internal class UpdateService : IUpdateService
    {
        private readonly IVersionService _versionService;
        private readonly HttpClient _httpClient;

        //private const string VersionUrl = "https://TU:URL/version.json";
        private const string VersionUrl = "C://User//JD66188//Downloads//version.json";

        public UpdateService(IVersionService versionService, HttpClient httpClient)
        {
            _versionService = versionService;
            _httpClient = httpClient;
        }

        public async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("version.json");

                using var reader = new StreamReader(stream);

                var json = await reader.ReadToEndAsync();

                var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(json);
                //var updateInfo = await _httpClient.GetFromJsonAsync<UpdateInfo>(VersionUrl);

                if (updateInfo == null)
                    return null;

                if (_versionService.IsNewerVersion(updateInfo.Version))
                    return updateInfo;

                return null;
            }
            catch (Exception ex)
            {
                //si no hay internet o falla github
                return null;
            }
        }
    }
}
