namespace Test.Commands
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

    public class DownloadHtmlResult : DownloadResult
    {
        private BytesConverter bytesConverter;
        public string Content { get; private set; }
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