using System;
using System.Collections.Generic;
using System.IO;
using YourNamespace;
using Item = YourNamespace.Item;

namespace HackBrowserData.BrowsingData
{
    public class Data
    {
        private readonly Dictionary<Item, Source> _sources;

        public Data(IEnumerable<Item> items)
        {
            _sources = new Dictionary<Item, Source>();
            AddSources(items);
        }

        public void Recovery(byte[] masterKey)
        {
            foreach (Source source in _sources.Values)
            {
                try
                {
                    source.Parse(masterKey);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Parse {source.Name()} error {ex.Message}");
                }
            }
        }

        public void Output(string dir, string browserName, string flag)
        {
            OutPutter output = new(flag);

            foreach (Source source in _sources.Values)
            {
                if (source.Len() == 0)
                {
                    // If the length of the export data is 0, then it is not necessary to output.
                    continue;
                }

                string filename = FileUtil.FileUtils.ItemName(browserName, source.Name(), output.Ext());

                try
                {
                    using FileStream f = output.CreateFile(dir, filename);
                    output.Write(source, f);
                }
                catch
                {
                }
            }
        }

        private void AddSources(IEnumerable<Item> items)
        {
            foreach (Item source in items)
            {
                switch (source)
                {
                    case Item.ChromiumPassword:
                        _sources[source] = new ChromiumPassword();
                        break;
                }
            }
        }
    }
}
