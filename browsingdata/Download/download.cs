using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace Download
{
    public class ChromiumDownload
    {
        public List<DownloadData> Downloads { get; } = new List<DownloadData>();

        public async Task ParseAsync()
        {
            string downloadPath = Item.TempChromiumDownload;
            try
            {
                if (File.Exists(downloadPath))
                {
                    using SQLiteConnection connection = new($"Data Source={downloadPath}");
                    await connection.OpenAsync();
                    string sql = "SELECT target_path, tab_url, total_bytes, start_time, end_time, mime_type FROM downloads";
                    using SQLiteCommand command = new(sql, connection);
                    using System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        DownloadData download = new()
                        {
                            TargetPath = reader.GetString(0),
                            URL = reader.GetString(1),
                            TotalBytes = reader.GetInt64(2),
                            StartTime = TypeUtil.TypeUtils.TimeEpoch(reader.GetInt64(3)),
                            EndTime = TypeUtil.TypeUtils.TimeEpoch(reader.GetInt64(4)),
                            MimeType = reader.GetString(5),
                        };
                        Downloads.Add(download);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing Chromium downloads: {ex.Message}");
            }
        }

        public string Name()
        {
            return "download";
        }

        public int Len()
        {
            return Downloads.Count;
        }
    }

    public class DownloadData
    {
        public string TargetPath { get; set; }
        public string URL { get; set; }
        public long TotalBytes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MimeType { get; set; }
    }

    public class Item
    {
        public const string TempChromiumDownload = "Path to Chromium Download file";
    }
}
