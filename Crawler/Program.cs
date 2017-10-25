using System;
using System.Collections.Generic;
using Crawler.Commands;

namespace Crawler
{
    class Program
    {
        public static void Main(string[] args)
        {
            PrintUsage("Starting");
            var fileSaver = new LocalDiskFileSaver();

            var webCrawler = new WebCrawler(UrlForProcessing, fileSaver);
            webCrawler.Process();

            ReadKeyboardAndPrintStatus(webCrawler.TriggeredForDownload, webCrawler.ProcessedDocuments);
            PrintUsage("Finished");
        }

        private static string UrlForProcessing = "https://tretton37.com";

        private static void PrintUsage(string StartingOrFinishing)
        {
            Console.WriteLine($"{StartingOrFinishing} with crawling of the site {UrlForProcessing}");
            Console.WriteLine(
                $"The files are donwloaded in the following location {FilePathGenerator.RootDirectoryForSavingFiles}");
            Console.WriteLine("Only the files that are not present (by file name) are downloaded");
            Console.WriteLine(
                $"Only the files form the same domain {UrlForProcessing} are processed , the other domains (external or e.g. http://tretton37img.blob.core.windows.net are ignored)!");
        }

        private static void ReadKeyboardAndPrintStatus(Dictionary<Uri, CrawlDocument> triggeredForDownload, Dictionary<Uri, CrawlDocument> processedDocuments)
        {
            Console.ReadLine();

            Console.WriteLine($"Triggered for download {triggeredForDownload.Values.Count} ");
            Console.WriteLine($"Downloaded {processedDocuments.Values.Count} ");
        }
    }
}
