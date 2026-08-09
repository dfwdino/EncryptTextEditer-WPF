using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace EncryptTextEditerCL
{
    public class FileIO
    {
        private static readonly byte[] VaultMarker = Encoding.ASCII.GetBytes("ETEVLT01");
        private const int VaultPbkdf2Iterations = 100_000;
        private const int VaultKeySize = 32;
        private const int VaultIvSize = 16;
        private const int VaultSaltSize = 16;
        private static string CryptKey = MakeKey();
        private static byte[] VI = MakeVI();

        private static byte[] MakeVI()
        {
            return Encoding.ASCII.GetBytes(PrintRandom(16));
        }

        public static void SaveFile(
            string location,
            string UnecryptedText,
            string CryptKey,
            byte[] VI
        )
        {
            string EncrptedData = AesOperation.EncryptString(CryptKey, VI, UnecryptedText);

            System.IO.File.WriteAllText(location, EncrptedData);
        }

        public static string LoadFile(string location, string key, byte[] VI)
        {
            string DecryptData = String.Empty;

            if (System.IO.File.Exists(location))
            {
                string EncrptedData = System.IO.File.ReadAllText(location);
                try
                {
                    DecryptData = AesOperation.DecryptString(key, VI, EncrptedData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Issue trying to decrypt file.  Error is {ex.Message}.  Program will not work."
                    );
                }
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
                CryptKey =
                    $"{Environment.MachineName}{Environment.UserDomainName}{Environment.UserName}"; // "b14ca5898a4e4133bbce2ea2315a1916";;

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

        public static byte[] GetVI()
        {
            return VI;
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
                var binaryFormatter =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Returns true if the file at filePath is a plain (pre-vault) BinaryFormatter file
        /// rather than a password-protected vault written by SaveVault.
        /// </summary>
        public static bool IsLegacyOptionsFile(string filePath)
        {
            try
            {
                ReadFromBinaryFile<object>(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Serializes objectToWrite and writes it to filePath encrypted with a key derived
        /// from password via PBKDF2. Overwrites any existing file at filePath.
        /// </summary>
        public static void SaveVault<T>(string filePath, string password, T objectToWrite)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(VaultSaltSize);
            byte[] iv = RandomNumberGenerator.GetBytes(VaultIvSize);
            byte[] key = DeriveVaultKey(password, salt);

            byte[] plainBytes;
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(VaultMarker, 0, VaultMarker.Length);
                var binaryFormatter =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToWrite);
                plainBytes = memoryStream.ToArray();
            }

            byte[] cipherBytes;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                }
            }

            using (Stream stream = File.Open(filePath, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(salt.Length);
                writer.Write(salt);
                writer.Write(iv.Length);
                writer.Write(iv);
                writer.Write(cipherBytes);
            }
        }

        /// <summary>
        /// Reads and decrypts a vault written by SaveVault. Throws WrongPasswordException
        /// if password is incorrect.
        /// </summary>
        public static T OpenVault<T>(string filePath, string password)
        {
            byte[] salt;
            byte[] iv;
            byte[] cipherBytes;

            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                using (var reader = new BinaryReader(stream))
                {
                    int saltLength = reader.ReadInt32();
                    salt = reader.ReadBytes(saltLength);
                    int ivLength = reader.ReadInt32();
                    iv = reader.ReadBytes(ivLength);
                    cipherBytes = reader.ReadBytes((int)(stream.Length - stream.Position));
                }
            }
            catch (Exception ex) when (ex is IOException or ArgumentException or OverflowException)
            {
                throw new VaultCorruptException(ex);
            }

            byte[] key = DeriveVaultKey(password, salt);
            byte[] plainBytes;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        plainBytes = decryptor.TransformFinalBlock(
                            cipherBytes,
                            0,
                            cipherBytes.Length
                        );
                    }
                }
            }
            catch (CryptographicException)
            {
                throw new WrongPasswordException();
            }

            if (
                plainBytes.Length < VaultMarker.Length
                || !plainBytes.AsSpan(0, VaultMarker.Length).SequenceEqual(VaultMarker)
            )
            {
                throw new WrongPasswordException();
            }

            try
            {
                using (var memoryStream = new MemoryStream(plainBytes, VaultMarker.Length, plainBytes.Length - VaultMarker.Length))
                {
                    var binaryFormatter =
                        new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (T)binaryFormatter.Deserialize(memoryStream);
                }
            }
            catch (Exception ex) when (ex is not (WrongPasswordException or VaultCorruptException))
            {
                throw new VaultCorruptException(ex);
            }
        }

        private static byte[] DeriveVaultKey(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                VaultPbkdf2Iterations,
                HashAlgorithmName.SHA256,
                VaultKeySize
            );
        }
    }
}
