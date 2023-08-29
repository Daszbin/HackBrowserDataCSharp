using System;
using System.Collections.Generic;
using System.IO;

namespace Extension
{
    public class ChromiumExtension
    {
        public List<ExtensionData> Extensions { get; } = new List<ExtensionData>();

        public void Parse()
        {
            string extensionFolder = item.TempChromiumExtension;
            try
            {
                if (Directory.Exists(extensionFolder))
                {
                    string[] manifestFiles = Directory.GetFiles(extensionFolder, "manifest.json", SearchOption.AllDirectories);
                    foreach (string manifestFile in manifestFiles)
                    {
                        string content = File.ReadAllText(manifestFile);
                        ExtensionData extension = new ExtensionData
                        {
                            Name = GetValueFromJSON(content, "name"),
                            Description = GetValueFromJSON(content, "description"),
                            Version = GetValueFromJSON(content, "version"),
                            HomepageURL = GetValueFromJSON(content, "homepage_url")
                        };
                        Extensions.Add(extension);
                    }
                    Directory.Delete(extensionFolder, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing Chromium extensions: {ex.Message}");
            }
        }

        private string GetValueFromJSON(string json, string key)
        {
            string pattern = $"\"{key}\":\\s*\"(.*?)\"";
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(json, pattern);
            return match.Success && match.Groups.Count > 1 ? match.Groups[1].Value : string.Empty;
        }

        public string Name()
        {
            return "extension";
        }

        public int Len()
        {
            return Extensions.Count;
        }
    }

    public class ExtensionData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string HomepageURL { get; set; }
    }

    public class item
    {
        public const string TempChromiumExtension = "Path to Chromium Extension folder";
    }
}
