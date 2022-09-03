using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EncryptTextEditerCL
{
    public class FileIO
    {
        private static string CryptKey = MakeKey();

        public static void SaveFile(string location, string UnecryptedText)
        {
            string EncrptedData = AesOperation.EncryptString(CryptKey, UnecryptedText);

            System.IO.File.WriteAllText(location, EncrptedData);
        }

        public static string LoadFile(string location, string key)
        {
            string DecryptData = String.Empty;

            if (System.IO.File.Exists(location))
            {
                string EncrptedData = System.IO.File.ReadAllText(location);
                try
                {
                    DecryptData = AesOperation.DecryptString(key, EncrptedData);
                }

                catch (Exception ex) { MessageBox.Show($"Issue trying to decrypt file.  Error is {ex.Message}.  Program will not work."); }

            }

            return DecryptData;


        }


        public static string OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;

        }

        private static string MakeKey()
        {
            string CryptKey = string.Empty;

            if (string.IsNullOrEmpty(CryptKey))
            {
                CryptKey = $"{Environment.MachineName}{Environment.UserDomainName}{Environment.UserName}"; // "b14ca5898a4e4133bbce2ea2315a1916";;

                CryptKey += PrintRandom(32 - CryptKey.Length);

            }

            return CryptKey;
        }

        private static string PrintRandom(int RandomLength)
        {
            Random r = new Random();

            var sb = new StringBuilder();

            for (int i = 0; i < RandomLength; i++)
            {
                // Decide whether to add an uppercase letter, a lowercase letter, or a number
                int whichType = r.Next(0, 3);
                switch (whichType)
                {
                    // Lower case letter
                    case 0:
                        sb.Append((char)(97 + r.Next(0, 26)));
                        break;
                    // Upper case letter
                    case 1:
                        sb.Append((char)(65 + r.Next(0, 26)));
                        break;
                    // Number
                    case 2:
                        sb.Append((char)(48 + r.Next(0, 10)));
                        break;
                }
            }

            return sb.ToString();

        }

        public static string GetKey()
        {
            return CryptKey;

        }



        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }

        }
    }
}
