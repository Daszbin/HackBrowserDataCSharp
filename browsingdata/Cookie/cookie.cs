using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cookie
{
    public class ChromiumCookie
    {
        public List<Cookie> Cookies { get; } = new List<Cookie>();

        public async Task ParseAsync(byte[] masterKey)
        {
            string cookiesPath = Item.TempChromiumCookie;
            try
            {
                if (File.Exists(cookiesPath))
                {
                    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={cookiesPath}"))
                    {
                        await connection.OpenAsync();
                        string sql = "SELECT name, encrypted_value, host_key, path, creation_utc, expires_utc, is_secure, is_httponly, has_expires, is_persistent FROM cookies";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            using (System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Cookie cookie = new Cookie
                                    {
                                        KeyName = reader.GetString(0),
                                        Host = reader.GetString(2),
                                        Path = reader.GetString(3),
                                        IsSecure = Convert.ToBoolean(reader.GetInt32(6)),
                                        IsHTTPOnly = Convert.ToBoolean(reader.GetInt32(7)),
                                        HasExpire = Convert.ToBoolean(reader.GetInt32(8)),
                                        IsPersistent = Convert.ToBoolean(reader.GetInt32(9)),
                                        CreateDate = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(4)).DateTime,
                                        ExpireDate = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(5)).DateTime,
                                    };

                                    byte[] encryptedValue = (byte[])reader["encrypted_value"];
                                    string decryptedValue = DecryptValue(masterKey, encryptedValue);
                                    cookie.Value = decryptedValue;

                                    Cookies.Add(cookie);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing Chromium cookies: {ex.Message}");
            }
        }

        private string DecryptValue(byte[] masterKey, byte[] encryptedValue)
        {
            if (encryptedValue == null || encryptedValue.Length == 0)
            {
                return null;
            }

            try
            {
                if (masterKey == null || masterKey.Length == 0)
                {
                    return Encoding.UTF8.GetString(encryptedValue);
                }

                // Replace with your decryption logic using the masterKey
                // For example, you can use the AesManaged class:
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = masterKey;
                    aesAlg.IV = new byte[aesAlg.BlockSize / 8];
                    using (MemoryStream msDecrypt = new MemoryStream(encryptedValue))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting value: {ex.Message}");
                return null;
            }
        }
    }

    public class Cookie
    {
        public string Host { get; set; }
        public string Path { get; set; }
        public string KeyName { get; set; }
        public string Value { get; set; }
        public bool IsSecure { get; set; }
        public bool IsHTTPOnly { get; set; }
        public bool HasExpire { get; set; }
        public bool IsPersistent { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }

    public class Item
    {
        public const string TempChromiumCookie = "Path to Chromium Cookie file";
    }
}
