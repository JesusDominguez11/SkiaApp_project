using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaApp.Services
{
    public interface IInstallerService
    {
        Task InstallAsync(string apkPath);
    }
}
