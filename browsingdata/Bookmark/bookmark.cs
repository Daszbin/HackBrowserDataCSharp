using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace Bookmark
{
    public class ChromiumBookmark
    {
        public List<Bookmark> Bookmarks { get; } = new List<Bookmark>();

        public async Task ParseAsync(byte[] data)
        {
            string bookmarksPath = Item.TempChromiumBookmark;
            try
            {
                if (File.Exists(bookmarksPath))
                {
                    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={bookmarksPath}"))
                    {
                        await connection.OpenAsync();
                        string sql = "SELECT * FROM bookmarks";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            using (System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Bookmark bookmark = new Bookmark
                                    {
                                        ID = reader.GetInt64(0),
                                        Name = reader.GetString(1),
                                        Type = reader.GetString(2),
                                        URL = reader.GetString(3),
                                        DateAdded = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(4)).DateTime,
                                    };
                                    Bookmarks.Add(bookmark);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing Chromium bookmarks: {ex.Message}");
            }
        }
    }

    public class FirefoxBookmark
    {
        public List<Bookmark> Bookmarks { get; } = new List<Bookmark>();

        public async Task ParseAsync(byte[] data)
        {
            string firefoxBookmarkPath = Item.TempFirefoxBookmark;
            try
            {
                if (File.Exists(firefoxBookmarkPath))
                {
                    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={firefoxBookmarkPath}"))
                    {
                        await connection.OpenAsync();
                        string sql = "SELECT id, url, type, dateAdded, title FROM moz_bookmarks " +
                                  "INNER JOIN moz_places ON moz_bookmarks.fk = moz_places.id";
                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            using (System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Bookmark bookmark = new Bookmark
                                    {
                                        ID = reader.GetInt64(0),
                                        URL = reader.GetString(1),
                                        Type = LinkType(reader.GetInt64(2)),
                                        DateAdded = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(3) / 1000000).DateTime,
                                        Name = reader.GetString(4),
                                    };
                                    Bookmarks.Add(bookmark);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing Firefox bookmarks: {ex.Message}");
            }
        }

        private string LinkType(long a)
        {
            return a == 1 ? "url" : "folder";
        }
    }

    public class Bookmark
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string URL { get; set; }
        public DateTime DateAdded { get; set; }
    }

    public class Item
    {
        public const string TempChromiumBookmark = "Path to Chromium Bookmark file";
        public const string TempFirefoxBookmark = "Path to Firefox Bookmark file";
    }
}
