using CommandLine;
using System;
using System.Collections.Generic;
using YourNamespace;

internal class Program
{
    private static void Main(string[] args)
    {
        // Create an instance of BrowserPicker with your Chromium list
        List<Chromium> chromiumList = new List<Chromium>(); // Replace with your Chromium instances
        BrowserPicker browserPicker = new BrowserPicker(chromiumList);

        _ = Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                options.Browser = "edge";

                // Call the Execute method with parsed options and the browserPicker instance
                Execute(options, browserPicker);
            })
            .WithNotParsed(errors =>
            {
                // Handle parsing errors, if any
                Console.WriteLine("Invalid command-line arguments. Please check your input.");
            });
    }

    private static void Execute(Options options, BrowserPicker browserPicker)
    {
        List<IBrowser> browsers = browserPicker.PickBrowsers(options.Browser, options.ProfilePath);
        foreach (IBrowser browser in browsers)
        {
            try
            {
                HackBrowserData.BrowsingData.Data data = browser.GetBrowsingData(options.FullExport);
                data.Output(options.ResultsDir, browser.Name, options.Format);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                continue;
            }
        }

        if (options.Compress)
        {
            try
            {
                FileUtil.FileUtils.CompressDir(options.ResultsDir);
                Log.Notice("Compress success");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}

internal class Options
{
    [Option('v', "verbose", Default = false, HelpText = "Verbose")]
    public bool Verbose { get; set; }

    [Option('c', "compress", Default = false, HelpText = "Compress result to zip")]
    public bool Compress { get; set; }

    [Option('b', "browser", Default = "all", HelpText = "Available browsers: all|chrome|firefox|edge")]
    public string Browser { get; set; }

    [Option('r', "results-dir", Default = "results", HelpText = "Export directory")]
    public string ResultsDir { get; set; }

    [Option('o', "format", Default = "csv", HelpText = "File format: csv|json")]
    public string Format { get; set; }

    [Option('p', "profile-path", Default = "", HelpText = "Custom profile directory path, get with chrome://version")]
    public string ProfilePath { get; set; }

    [Option('x', "full-export", Default = true, HelpText = "Export full browsing data")]
    public bool FullExport { get; set; }
}
