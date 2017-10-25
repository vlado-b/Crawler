using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Test;
using Test.Commands;

namespace Crawler
{
    class WebCrawler
    {
        private readonly FileSaver fileSaver; 

        public WebCrawler(string url, FileSaver fileSaver)
        {
            urlForProcessing = url;
            this.fileSaver = fileSaver;
            ProcessedDocuments = new Dictionary<Uri, CrawlDocument>();
            TriggeredForDownload = new Dictionary<Uri, CrawlDocument>();
        }

        public string urlForProcessing;
        public Dictionary<Uri, CrawlDocument> ProcessedDocuments;
        public Dictionary<Uri, CrawlDocument> TriggeredForDownload;


        public void Process( )
        {
            


            var commands = new Subject<Command>();

            var commandStream = commands.Do(c => Console.WriteLine($"Command added to queue {c}"))
                .SelectMany(c => Observable.FromAsync(x => c.Execute()));

            var producerAbstraction = commandStream.Publish();

            producerAbstraction
                //.Do(r => Console.WriteLine($@"result from executed command {r}"))
                .Subscribe(c =>
                {
                    Console.WriteLine($"Result from Executed command {c}");
                    
                });

            producerAbstraction
                .OfType<DownloadResult>().Where(c=>c.IsSucess).Subscribe(
                    c =>
                    {
                        if (!ProcessedDocuments.ContainsKey(c.Document.Uri))
                        {
                            ProcessedDocuments.Add(c.Document.Uri, c.Document);
                        }
                    

                        var saveDocumentCmd = CreateSaveDocumentCommand(c);
                        commands.OnNext(saveDocumentCmd);

                        if( c is DownloadHtmlResult )
                        {
                            var parseCommand = CreateLinkParserCommand(c as DownloadHtmlResult );
                            commands.OnNext(parseCommand);
                        }
                    }
                );

            

            producerAbstraction
                .OfType<ParseResult>().Where(c=>c.IsSucess).Subscribe(
                    c =>
                    {
                        foreach (var crawlDocument in c.ParsedLinks)
                        {
                            if( ! ProcessedDocuments.ContainsKey( crawlDocument.Uri )
                                &&  !TriggeredForDownload.ContainsKey(crawlDocument.Uri))
                            {
                                TriggeredForDownload.Add(crawlDocument.Uri, crawlDocument);
                                var cmd = CreateDownloadCommand(crawlDocument);
                                commands.OnNext(cmd);
                            }
                        }

                    }
                );

            producerAbstraction.Connect();

            var firstDocumentForDownload = new CrawlDocument(new Uri(urlForProcessing),false );
            var initiatingCommandForRoot = new DownloadHtmlDocumentCmd(firstDocumentForDownload);
            TriggeredForDownload.Add(  firstDocumentForDownload.Uri , firstDocumentForDownload  );
            commands.OnNext(initiatingCommandForRoot);

        }

        private SaveDocumentCmd CreateSaveDocumentCommand(DownloadResult downloadResult)
        {
            var command = new SaveDocumentCmd( new SaveDocumentRequest(downloadResult.Document.Uri,downloadResult.GetDocumentContent()  ), fileSaver);

            return command;
        }

        private DownloadDocumentCmd CreateDownloadCommand(CrawlDocument crawlDocument)
        {
            if ( crawlDocument.IsImage )
                return new DownloadImageDocumentCmd(crawlDocument);
            else 
                return new DownloadHtmlDocumentCmd(crawlDocument);
        }

        private LinkParserCmd CreateLinkParserCommand(DownloadHtmlResult c)
        {
            return new LinkParserCmd(c.Content, c.Document.Uri);
        }
    }

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
