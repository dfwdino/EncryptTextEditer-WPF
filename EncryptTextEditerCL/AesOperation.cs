using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptTextEditerCL
{
    public class AesOperation
    {
        public static string EncryptString(string key, byte[] VI, string plainText)
        {
            byte[] array;

            using (Aes aes = Aes.Create())
            {

                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = VI;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (
                        CryptoStream cryptoStream = new CryptoStream(
                            (Stream)memoryStream,
                            encryptor,
                            CryptoStreamMode.Write
                        )
                    )
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                            //cryptoStream.FlushFinalBlock();
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, byte[] VI, string cipherText)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = VI;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (
                        CryptoStream cryptoStream = new CryptoStream(
                            (Stream)memoryStream,
                            decryptor,
                            CryptoStreamMode.Read
                        )
                    )
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
