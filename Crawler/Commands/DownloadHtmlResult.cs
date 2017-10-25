using Crawler.Utils;

namespace Crawler.Commands
{
    public class DownloadHtmlResult : DownloadResult
    {
        private readonly BytesConverter bytesConverter;
        public string Content { get; }
        public DownloadHtmlResult(string content, bool isSucess, CrawlDocument document)
        {
            Content = content;
            Document = document;
            IsSucess = isSucess;
            bytesConverter = new BytesConverter();
        }

        public override byte[] GetDocumentContent()
        {
            return bytesConverter.GetBytes(Content);
        }
    }
}