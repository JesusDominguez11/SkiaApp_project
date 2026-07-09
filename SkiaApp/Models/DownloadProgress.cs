using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaApp.Models
{
    public class DownloadProgress
    {
        public long BytesReceived { get; set; }

        public long TotalBytes { get; set; }

        public double Percentage =>
            TotalBytes == 0
                ? 0
                : (double)BytesReceived / TotalBytes;
    }
}
