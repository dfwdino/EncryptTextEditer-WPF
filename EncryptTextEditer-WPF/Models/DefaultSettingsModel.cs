using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptTextEditer_WPF.Models
{
    internal class DefaultSettingsModel
    {
        public static readonly string FolderDefaultLocation = System.IO.Directory.GetCurrentDirectory();
        public static readonly string FileDailyName = $"\\{DateTime.Now.ToString("yyyyMMdd")}.txt";
        public static readonly string FileOneTimeUseName = "\\textfile.txt";

        public static string FullDefaultLocation = string.Empty;

        public static readonly string OptionsFileLocation = System.IO.Directory.GetCurrentDirectory() + "\\options.txt";
    }
}
