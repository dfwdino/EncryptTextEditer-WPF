using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptTextEditer_WPF.Models
{
    [Serializable]
    internal class OptionModel
    {
        public bool UseDailyFile { get; set; } = false;

        public string CustomKey { get; set; } = string.Empty;

        public byte[] CustomVI = new byte[0];
    }
}
