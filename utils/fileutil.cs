using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace FileUtil
{
    public class FileUtils
    {
        public static bool IsFileExists(string filename)
        {
            return File.Exists(filename);
        }

        public static bool IsDirExists(string folder)
        {
            return Directory.Exists(folder);
        }

        public static List<string> FilesInFolder(string dir, string filename)
        {
            if (!IsDirExists(dir))
            {
                throw new IOException(dir + " folder does not exist");
            }

            List<string> files = new();
            foreach (string filePath in Directory.EnumerateFiles(dir, filename, SearchOption.AllDirectories))
            {
                files.Add(filePath);
            }
            return files;
        }

        public static string ReadFile(string filename)
        {
            return File.ReadAllText(filename);
        }

        public static void CopyDir(string src, string dst, string skip)
        {
            foreach (string sourcePath in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
            {
                string targetPath = Path.Combine(dst, sourcePath[src.Length..]);
                _ = Directory.CreateDirectory(targetPath);
            }

            foreach (string sourcePath in Directory.GetFiles(src, "*.*", SearchOption.AllDirectories)
                .Where(path => !path.EndsWith(skip, StringComparison.OrdinalIgnoreCase)))
            {
                string targetPath = Path.Combine(dst, sourcePath[src.Length..]);
                File.Copy(sourcePath, targetPath, true);
            }
        }

        public static void CopyDirHasSuffix(string src, string dst, string suffix)
        {
            List<string> files = new();

            foreach (string filePath in Directory.EnumerateFiles(src, "*" + suffix, SearchOption.AllDirectories))
            {
                files.Add(filePath);
            }

            _ = Directory.CreateDirectory(dst);

            for (int index = 0; index < files.Count; index++)
            {
                string sourcePath = files[index];
                string fileName = Path.GetFileName(sourcePath);
                string targetPath = Path.Combine(dst, $"{index}_{fileName}");
                File.Copy(sourcePath, targetPath);
            }
        }

        public static void CopyFile(string src, string dst)
        {
            File.Copy(src, dst);
        }

        public static string ItemName(string browser, string item, string ext)
        {
            string replace = browser.Replace(" ", "_").Replace(".", "_").Replace("-", "_");
            return $"{replace.ToLower()}_{item}.{ext}";
        }

        public static string BrowserName(string browser, string user)
        {
            string replace = browser.Replace(" ", "_").Replace(".", "_").Replace("-", "_").Replace("Profile", "user");
            return $"{replace.ToLower()}_{replace.ToLower()}";
        }

        public static string ParentDir(string p)
        {
            return Path.GetDirectoryName(Path.GetFullPath(p));
        }

        public static string BaseDir(string p)
        {
            return Path.GetFileName(p);
        }

        public static string ParentBaseDir(string p)
        {
            return BaseDir(ParentDir(p));
        }
        public static void Walk(string root, Action<string, FileInfo> fileAction)
        {
            var files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories);

            foreach (var filePath in files)
            {
                var fileInfo = new FileInfo(filePath);
                fileAction(filePath, fileInfo);
            }
        }
        public static void CompressDir(string dir)
        {
            string zipFileName = $"{dir}.zip";

            using FileStream zipToOpen = new(zipFileName, FileMode.Create);
            using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Create);
            foreach (string filePath in Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories))
            {
                string entryName = filePath[dir.Length..].Replace('\\', '/');
                ZipArchiveEntry entry = archive.CreateEntry(entryName);

                using (FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read))
                using (Stream entryStream = entry.Open())
                {
                    fileStream.CopyTo(entryStream);
                }

                File.Delete(filePath);
            }
        }
    }
}
