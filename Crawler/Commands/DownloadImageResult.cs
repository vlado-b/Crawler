namespace Crawler.Commands
    {
    public class DownloadImageResult : DownloadResult
    {

        public byte[] Bytes { get; }

        public DownloadImageResult(byte[] bytes, bool isSucess, CrawlDocument document)
        {
            Bytes = bytes;
            IsSucess = isSucess;
            Document = document;
        }

        public override byte[] GetDocumentContent()
        {
            return Bytes;
        }
    }
}