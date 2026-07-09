using SkiaApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaApp.Services
{
    public interface IUpdateService
    {
        Task<UpdateInfo?> CheckForUpdatesAsync();
    }
}
