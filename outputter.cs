using CsvHelper;
using CsvHelper.Configuration;
using HackBrowserData.BrowsingData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace YourNamespace
{
    public class OutPutter
    {
        private readonly bool _json;
        private readonly bool _csv;

        public OutPutter(string flag)
        {
            if (flag == "json")
            {
                _json = true;
            }
            else
            {
                _csv = true;
            }
        }

        public void Write(Source data, Stream writer)
        {// Assuming your Source objects can be converted to a collection (e.g., a list)
            IEnumerable<Source> sourceCollection = new List<Source> { data }; // Create a collection containing your data

            if (_json)
            {
                // JSON serialization code remains the same
                using Utf8JsonWriter writerUtf8 = new(writer);
                JsonSerializerOptions options = new()
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                JsonSerializer.Serialize(writerUtf8, sourceCollection, options);
            }
            else if (_csv)
            {
                CsvConfiguration config = new(CultureInfo.CurrentCulture)
                {
                    ShouldQuote = field => true,
                };
                using CsvWriter csvWriter = new(new StreamWriter(writer), config);
                csvWriter.WriteRecords(sourceCollection); // Pass the collection here
            }

        }

        public FileStream CreateFile(string dir, string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("Empty filename");
            }

            if (!string.IsNullOrEmpty(dir))
            {
                if (!Directory.Exists(dir))
                {
                    _ = Directory.CreateDirectory(dir);
                }
            }

            string filePath = Path.Combine(dir, filename);
            return new FileStream(filePath, FileMode.Create, FileAccess.Write);
        }

        public string Ext()
        {
            return _json ? "json" : "csv";
        }
    }
}
