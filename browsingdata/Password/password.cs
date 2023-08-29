using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Password
{
    public class ChromiumPassword : List<LoginData>
    {
        private const string QueryChromiumLogin = "SELECT origin_url, username_value, password_value, date_created FROM logins";

        public void Parse(byte[] masterKey)
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={Item.TempChromiumPassword};Version=3;"))
            {
                db.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(QueryChromiumLogin, db))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string url = reader.GetString(0);
                            string username = reader.GetString(1);
                            byte[] pwd = reader.IsDBNull(2) ? null : (byte[])reader[2];
                            long create = reader.GetInt64(3);

                            LoginData login = new LoginData
                            {
                                UserName = username,
                                EncryptPass = pwd,
                                LoginURL = url
                            };

                            if (pwd != null && pwd.Length > 0)
                            {
                                byte[] password = masterKey == null || masterKey.Length == 0 ? Crypto.DPAPI(pwd) : Crypto.DecryptPass(masterKey, pwd);
                                login.Password = Encoding.UTF8.GetString(password);
                            }

                            login.CreateDate = create > DateTimeOffset.Now.ToUnixTimeSeconds()
                                ? TypeUtil.TimeEpoch(create)
                                : TypeUtil.TimeStamp(create);

                            Add(login);
                        }
                    }
                }
            }

            Sort((a, b) => b.CreateDate.CompareTo(a.CreateDate));
        }

        public string Name()
        {
            return "password";
        }
    }

    public class LoginData
    {
        public string UserName { get; set; }
        public byte[] EncryptPass { get; set; }
        public byte[] EncryptUser { get; set; }
        public string Password { get; set; }
        public string LoginURL { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }

    public class Crypto
    {
        public static byte[] DPAPI(byte[] data)
        {
            // Implement DPAPI decryption logic here
            throw new NotImplementedException();
        }

        public static byte[] DecryptPass(byte[] masterKey, byte[] data)
        {
            // Implement decryption logic here
            throw new NotImplementedException();
        }
    }

    public static class TypeUtil
    {
        public static DateTimeOffset TimeEpoch(long epoch)
        {
            return DateTimeOffset.FromUnixTimeSeconds(epoch);
        }

        public static DateTimeOffset TimeStamp(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp * 1000);
        }
    }

    public static class Item
    {
        public const string TempChromiumPassword = "PathToChromiumPasswordDatabase";
    }
}
