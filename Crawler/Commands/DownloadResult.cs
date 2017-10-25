namespace Crawler.Commands
{
    public abstract class DownloadResult : Result
    {
        public CrawlDocument Document { get; protected set; }

        public override string ToString()
        {
            return $"DownloadResult  for URI {Document.Uri} status: {IsSucess}";
        }

        public abstract byte[] GetDocumentContent();
    }
}