using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YourNamespace;

public interface IBrowser
{
    string Name { get; }
    HackBrowserData.BrowsingData.Data GetBrowsingData(bool isFullExport);
}

public class BrowserPicker(List<Chromium> chromiumList)
{
    private readonly List<Chromium> _chromiumList = chromiumList;

    public List<IBrowser> PickBrowsers(string name, string profile)
    {
        List<IBrowser> browsers = new();
        name = name.ToLower();

        if (name == "all")
        {
            foreach (Chromium chromium in _chromiumList)
            {
                if (!Directory.Exists(chromium._profilePath))
                {
                    Console.WriteLine($"Find browser {chromium._name} failed, profile folder does not exist");
                    continue;
                }

                List<Chromium> multiChromium = Chromium.New(chromium._name, chromium._storage, chromium._profilePath, chromium._items);

                foreach (Chromium b in multiChromium)
                {
                    Console.WriteLine($"Find browser {b._name} success");
                    browsers.Add((IBrowser)b);
                }
            }
        }

        if (_chromiumList.Any(c => c._name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            Chromium c = _chromiumList.First(c => c._name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(profile))
            {
                profile = c._profilePath;
            }

            if (!Directory.Exists(profile))
            {
                Console.WriteLine($"Find browser {c._name} failed, profile folder does not exist");
            }
            else
            {
                List<Chromium> chromiumList = Chromium.New(c._name, c._storage, profile, c._items);

                foreach (Chromium b in chromiumList)
                {
                    Console.WriteLine($"Find browser {b._name} success");
                    browsers.Add((IBrowser)b);
                }
            }
        }
        return browsers;
    }
}
