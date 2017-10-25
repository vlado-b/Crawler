using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Crawler.Commands;

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
            commands = new Subject<Command>();
        }

        public string urlForProcessing;
        public Dictionary<Uri, CrawlDocument> ProcessedDocuments;
        public Dictionary<Uri, CrawlDocument> TriggeredForDownload;
        private readonly Subject<Command> commands;
        private IObservable<Result> commandStream;
        private IConnectableObservable<Result> producerAbstraction;


        public void Process( )
        {
            BindCommandStream();

            producerAbstraction = commandStream.Publish();

            BindMainSubscriberForAllCommandResults();

            BindSubscriberForDownloadResult();

            BindSubscriberForParseResult();

            producerAbstraction.Connect();

            StartFlowByQueuingFirstCommand();
        }

        private void StartFlowByQueuingFirstCommand()
        {
            var firstDocumentForDownload = new CrawlDocument(new Uri(urlForProcessing), false);
            var initiatingCommandForRoot = new DownloadHtmlDocumentCmd(firstDocumentForDownload);
            TriggeredForDownload.Add(firstDocumentForDownload.Uri, firstDocumentForDownload);
            commands.OnNext(initiatingCommandForRoot);
        }

        private void BindSubscriberForParseResult()
        {
            producerAbstraction
                .OfType<ParseResult>().Where(c => c.IsSucess).Subscribe(
                    c =>
                    {
                        foreach (var crawlDocument in c.ParsedLinks)
                        {
                            QueueDownloadForDocumentIfNotAlreadyTriggeredOrProcessed(crawlDocument);
                        }
                    }
                );
        }

        private void QueueDownloadForDocumentIfNotAlreadyTriggeredOrProcessed(CrawlDocument crawlDocument)
        {
            if (!ProcessedDocuments.ContainsKey(crawlDocument.Uri)
                && !TriggeredForDownload.ContainsKey(crawlDocument.Uri))
            {
                TriggeredForDownload.Add(crawlDocument.Uri, crawlDocument);
                var cmd = CreateDownloadCommand(crawlDocument);
                commands.OnNext(cmd);
            }
        }

        private void BindSubscriberForDownloadResult()
        {
            producerAbstraction
                .OfType<DownloadResult>().Where(c => c.IsSucess).Subscribe(
                    c =>
                    {
                        AddDocumentToProcessedIfNotPreasent(c);

                        QueueSaveOfDocument(c);

                        QueueParsingOfHtmlResult(c);
                    }
                );
        }

        private void QueueParsingOfHtmlResult(DownloadResult c)
        {
            if (c is DownloadHtmlResult)
            {
                var parseCommand = CreateLinkParserCommand(c as DownloadHtmlResult);
                commands.OnNext(parseCommand);
            }
        }

        private void QueueSaveOfDocument(DownloadResult c)
        {
            var saveDocumentCmd = CreateSaveDocumentCommand(c);
            commands.OnNext(saveDocumentCmd);
        }

        private void AddDocumentToProcessedIfNotPreasent(DownloadResult c)
        {
            if (!ProcessedDocuments.ContainsKey(c.Document.Uri))
            {
                ProcessedDocuments.Add(c.Document.Uri, c.Document);
            }
        }

        private void BindMainSubscriberForAllCommandResults()
        {
            producerAbstraction
                .Subscribe(c => { Console.WriteLine($"Result from Executed command {c}"); });
        }

        private void BindCommandStream()
        {
            commandStream = commands.Do(c => Console.WriteLine($"Command added to queue {c}"))
                .SelectMany(c => Observable.FromAsync(x => c.Execute()));
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
}