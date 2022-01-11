using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptTextEditerCL
{
    public class FileIO
    {
        private static string CryptKey = "b14ca5898a4e4133bbce2ea2315a1916";


        public static void SaveFile(string location, string value)
        {
            string EncrptedData = AesOperation.EncryptString(CryptKey, value);

            System.IO.File.WriteAllText(location, EncrptedData);
        }

        public static string LoadFile(string location)
        {
            string DecryptData = String.Empty;

            if (System.IO.File.Exists(location))
            {
                string EncrptedData = System.IO.File.ReadAllText(location);

                DecryptData = AesOperation.DecryptString(CryptKey, EncrptedData);
            }
            
            return DecryptData;


        }


        public static string OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            return openFileDialog.ShowDialog() == true ?  openFileDialog.FileName : string.Empty;

            //if (openFileDialog.ShowDialog() == true)
            //{
            //    return openFileDialog.FileName;
                
            //}
        }


    }
}
