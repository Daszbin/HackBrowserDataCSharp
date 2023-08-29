using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace History
{
    public class ChromiumHistory
    {
        public List<HistoryData> HistoryEntries { get; } = new List<HistoryData>();

        public void Parse()
        {
            string historyFile = Item.TempChromiumHistory;
            try
            {
                if (File.Exists(historyFile))
                {
                    using (SQLiteConnection connection = new($"Data Source={historyFile};Version=3;"))
                    {
                        connection.Open();
                        string query = "SELECT url, title, visit_count, last_visit_time FROM urls";
                        using SQLiteCommand command = new(query, connection);
                        using SQLiteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string url = reader["url"].ToString();
                            string title = reader["title"].ToString();
                            int visitCount = int.Parse(reader["visit_count"].ToString());
                            long lastVisitTime = long.Parse(reader["last_visit_time"].ToString());
                            HistoryData historyEntry = new()
                            {
                                URL = url,
                                Title = title,
                                VisitCount = visitCount,
                                LastVisitTime = TypeUtil.TypeUtils.TimeEpoch(lastVisitTime)
                            };
                            HistoryEntries.Add(historyEntry);
                        }
                    }
                    File.Delete(historyFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing Chromium history: {ex.Message}");
            }
        }

        public string Name()
        {
            return "history";
        }

        public int Len()
        {
            return HistoryEntries.Count;
        }
    }

    public class HistoryData
    {
        public string Title { get; set; }
        public string URL { get; set; }
        public int VisitCount { get; set; }
        public DateTime LastVisitTime { get; set; }
    }

    public class Item
    {
        public const string TempChromiumHistory = "Path to Chromium History database file";
    }
}
