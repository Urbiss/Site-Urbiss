using System;
using System.IO;
using System.Linq;

namespace Urbiss.Domain.Dtos
{
    public class AppSettingsDto
    {
        public string Url { get; set; }
        public string DataFolder { get; set; }
        public string OutputFolder { get; set; }
        public string TempFolder { get; set; }
        public string UploadFolder { get; set; }
        public int MaxAreaBounds { get; set; }
        public static string GetAppFolder(params string[] paths) => Path.Combine(new string[] { ".." }.Concat(paths).ToArray());
    }
}
