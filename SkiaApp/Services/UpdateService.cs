using SkiaApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const string VersionUrl = "https://raw.githubusercontent.com/JesusDominguez11/SkiaApp_project/refs/heads/master/SkiaApp/version.json";

        public UpdateService(IVersionService versionService, HttpClient httpClient)
        {
            _versionService = versionService;
            _httpClient = httpClient;
        }

        public async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            try
            { 
                var updateInfo = await _httpClient.GetFromJsonAsync<UpdateInfo>(VersionUrl);

                if (updateInfo == null)
                    return null;

                if (_versionService.IsNewerVersion(updateInfo.Build))
                    return updateInfo;

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //si no hay internet o falla github
                return null;
            }
        }
    }
}
