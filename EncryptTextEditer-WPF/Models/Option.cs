using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptTextEditer_WPF.Models
{
    [Serializable]
    internal class Option
    {
        public bool UseOneFile {get;set;} = false;

        public string CustomKey { get; set; } = string.Empty;
    }
}
