using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace SessionStorage
{
    public class ChromiumSessionStorage : List<Session>
    {
        private const string QueryChromiumSessionStorage = "SELECT key, value FROM ItemTable";

        public void Parse()
        {
            using (var db = new SQLiteConnection($"Data Source={Item.TempChromiumSessionStorage};Version=3;"))
            {
                db.Open();
                using (var cmd = new SQLiteCommand(QueryChromiumSessionStorage, db))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var key = reader.GetString(0);
                            var value = reader.IsDBNull(1) ? null : (byte[])reader[1];
                            var session = new Session();

                            if (value != null)
                            {
                                session.Key = key;
                                session.FillValue(value);
                            }
                            else
                            {
                                session.FillKey(key);
                            }

                            Add(session);
                        }
                    }
                }
            }
        }

        public string Name() => "sessionStorage";

        public int Len() => Count;
    }

    public class Session
    {
        public bool IsMeta { get; set; }
        public string URL { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public void FillKey(string key)
        {
            var keys = key.Split('-');
            if (keys.Length == 1 && keys[0].StartsWith("META:"))
            {
                IsMeta = true;
                FillMetaHeader(keys[0]);
            }
            else if (keys.Length == 2 && keys[0].StartsWith("_"))
            {
                FillHeader(keys[0], keys[1]);
            }
            else if (keys.Length == 3)
            {
                if (keys[0] == "map")
                {
                    Key = keys[2];
                }
                else if (keys[0] == "namespace")
                {
                    URL = keys[2];
                    Key = keys[1];
                }
            }
        }

        public void FillMetaHeader(string header)
        {
            URL = header.TrimStart("META:".ToCharArray());
        }

        public void FillHeader(string url, string key)
        {
            URL = url.TrimStart("_".ToCharArray());
            Key = key.TrimEnd("\x01".ToCharArray());
        }


        public void FillValue(byte[] value)
        {
            var decodedValue = Encoding.UTF8.GetString(value);
            Value = decodedValue;
        }
    }

    public static class Item
    {
        public const string TempChromiumSessionStorage = "PathToChromiumSessionStorage";
    }
}
