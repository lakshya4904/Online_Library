using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#pragma warning disable 618

namespace Entities
{
    public static class Config
    {
        public static string LogPath => Environment.CurrentDirectory + "/Logs";
        private static string EncryptionKey => "WE-NS-#2022$";

        static Config()
        {
            if(!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
        }
        public static string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder()
                {
                    DataSource = @"(LocalDB)\MSSQLLocalDB",
                    AttachDBFilename = $@"{Environment.CurrentDirectory}\DB_WeldEZ.mdf",
                    InitialCatalog = "WeldEZ",
                    IntegratedSecurity = true,
                    MultipleActiveResultSets = true,
                    UserInstance = false,
                    ConnectTimeout = 14400
                };
                return sqlBuilder.ToString();
            }
        }

        public static string Encrypt(string input)
        {
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;
            try
            {
                using(var aes = new RijndaelManaged())
                {
                    aes.KeySize = 128;
                    aes.BlockSize = 128;
                    var plainText = Encoding.Unicode.GetBytes(input);
                    var secretKey = new PasswordDeriveBytes(EncryptionKey,
                        Encoding.ASCII.GetBytes(EncryptionKey.Length.ToString()));
                    using(var encrypt = aes.CreateEncryptor(rgbKey: secretKey.GetBytes(16),
                        rgbIV: secretKey.GetBytes(16)))
                    {
                        using(memoryStream = new MemoryStream())
                        {
                            using(cryptoStream = new CryptoStream(memoryStream, encrypt, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainText, 0, plainText.Length);
                                cryptoStream.FlushFinalBlock();
                                return Convert.ToBase64String(memoryStream.ToArray());
                            }
                        }
                    }
                }

            }
            finally
            {
                memoryStream?.Close();
                cryptoStream?.Close();
            }
        }

        public static string Decrypt(string inputText)
        {
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;
            try
            {
                using(var aes = new RijndaelManaged())
                {
                    aes.KeySize = 128;
                    aes.BlockSize = 128;
                    var encryptedData = Convert.FromBase64String(inputText);
                    var secretKey = new PasswordDeriveBytes(EncryptionKey,
                        Encoding.ASCII.GetBytes(EncryptionKey.Length.ToString()));
                    using(var decrypt = aes.CreateDecryptor(secretKey.GetBytes(16),
                        secretKey.GetBytes(16)))
                    {
                        using(memoryStream = new MemoryStream(encryptedData))
                        {
                            using(cryptoStream = new CryptoStream(memoryStream, decrypt, CryptoStreamMode.Read))
                            {
                                var plainText = new byte[encryptedData.Length];
                                return Encoding.Unicode.GetString(plainText, 0, cryptoStream.Read(plainText, 0,
                                    plainText.Length));
                            }
                        }
                    }
                }

            }
            finally
            {
                memoryStream?.Close();
                cryptoStream?.Close();
            }
        }
    }
}