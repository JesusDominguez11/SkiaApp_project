using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace SkiaApp.Services
{
    internal class VersionService : IVersionService
    {
        public string CurrentVersion =>
            AppInfo.Current.VersionString;

        public int Build =>
            int.Parse(AppInfo.Current.BuildString);

        public bool IsNewerVersion(int remoteBuild)
        {
            return remoteBuild > Build;
        }
    }
}
