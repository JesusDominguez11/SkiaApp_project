using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaApp.Models
{
    public class UpdateInfo
    {
        public string Version { get; set; } = "";

        public int Build {  get; set; }
        public string ApkUrl { get; set; } = "";

        public string Changelog { get; set; } = "";

        public bool Mandatory { get; set; }

        public long Size { get; set; }
    }
}
