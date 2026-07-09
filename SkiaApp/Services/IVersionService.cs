using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaApp.Services
{
    public interface IVersionService
    {
        string CurrentVersion {  get; }
        int Build {  get; }
        bool IsNewerVersion(string remoteVersion);
    }
}
