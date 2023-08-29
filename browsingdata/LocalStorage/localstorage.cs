using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage
{
    public class ChromiumLocalStorage
    {
        public List<StorageData> StorageEntries { get; } = new List<StorageData>();

        public async Task ParseAsync()
        {
            string dbFile = Item.TempChromiumLocalStorage;
            if (File.Exists(dbFile))
            {
                using (SQLiteConnection dbConnection = new($"Data Source={dbFile};Version=3;"))
                {
                    await dbConnection.OpenAsync();
                    using SQLiteCommand dbCommand = dbConnection.CreateCommand();
                    dbCommand.CommandText = "SELECT key, value FROM ItemTable";
                    using System.Data.Common.DbDataReader reader = await dbCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        string key = Encoding.UTF8.GetString((byte[])reader["key"]);
                        string value = Encoding.UTF8.GetString((byte[])reader["value"]);

                        StorageData storageEntry = new()
                        {
                            IsMeta = false, // TODO: Implement meta detection
                            URL = "", // TODO: Implement URL extraction
                            Key = key,
                            Value = value
                        };

                        if (storageEntry.IsMeta)
                        {
                            storageEntry.Value = "meta data, value bytes is " + Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                        }

                        if (storageEntry.Key.StartsWith("_"))
                        {
                            string[] parts = storageEntry.Key.Split('_');
                            if (parts.Length == 2)
                            {
                                storageEntry.URL = parts[0];
                                storageEntry.Key = parts[1];
                            }
                        }

                        if (storageEntry.Value.Length >= 1024)
                        {
                            storageEntry.Value = $"value is too long, length is {storageEntry.Value.Length}, supported max length is {1024}";
                        }

                        StorageEntries.Add(storageEntry);
                    }
                }

                File.Delete(dbFile);
            }
        }

        public string Name()
        {
            return "localStorage";
        }

        public int Len()
        {
            return StorageEntries.Count;
        }
    }

    public class StorageData
    {
        public bool IsMeta { get; set; }
        public string URL { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Item
    {
        public const string TempChromiumLocalStorage = "Path to Chromium LocalStorage database file";
    }
}
