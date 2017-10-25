using System;
using System.IO;
using System.Reflection;

namespace Crawler
{
    public class FilePathGenerator
    {
        private static readonly string CrawlerDownloadsFolder;
        public static readonly string RootDirectoryForSavingFiles;

        static FilePathGenerator()
        {
            CrawlerDownloadsFolder = "Crawler_downloads";
            RootDirectoryForSavingFiles = Path.Combine(AssemblyDirectory, CrawlerDownloadsFolder);
        }

        public string GeneratePath(string localUriPath)
        {
            return GeneratePath(RootDirectoryForSavingFiles, localUriPath);
        }

        public string GeneratePath(string rootPath, string localUriPath)
        {
            
            localUriPath = GetDefaultFileName(localUriPath);
            localUriPath = ReplaceSlashWithBackSlash(localUriPath);

            localUriPath = RemoveRoutedPath(localUriPath);
            if (localUriPath.EndsWith(@"\"))
            {
                localUriPath = localUriPath.Substring(0, localUriPath.Length - 1);
            }
            localUriPath = AddDefaultExtensionForHtmlDocuments(localUriPath);

            return Path.Combine(rootPath, localUriPath);
        }

        private string AddDefaultExtensionForHtmlDocuments(string localUriPath)
        {
            if (!Path.HasExtension(localUriPath))
                localUriPath = localUriPath + ".html";
            return localUriPath;
        }

        private string GetDefaultFileName(string localUriPath)
        {
            if (string.IsNullOrWhiteSpace(localUriPath) || localUriPath == "/"   )
            {
                localUriPath = "index.html";
            }
            return localUriPath;
        }

        private string RemoveRoutedPath(string value)
        {
            if (value.StartsWith(@"\"))
                value = value.Substring(1);
            return value;
        }

        private string ReplaceSlashWithBackSlash(string val)
        {
            return val.Replace("/", @"\");
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}