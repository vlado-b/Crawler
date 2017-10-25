using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Crawler.Commands
{
    public abstract class DownloadDocumentCmd : Command
    {
        protected CrawlDocument document;

        public override string ToString()
        {
            return $"DownloadDocumentCommand for URI {document.Uri}" + 
                (document.IsImage ? " Image" :"" ); 
        }

        public Task<Result> Execute()
        {
            return null;
        }
    }

    public class DownloadHtmlDocumentCmd : DownloadDocumentCmd, Command
    {
        public DownloadHtmlDocumentCmd(CrawlDocument document)
        {
            this.document = document;
        }

        public new async Task<Result>  Execute()
        {
            var httpClient = new HttpClient();
            string content;

            try
            {
                content = await httpClient.GetStringAsync(document.Uri);
            }
            catch (Exception exe)
            {
                Console.WriteLine($"exception {exe.Message} when downloading {document.Uri}");
                return await ReturnError();
            }

            return await ReturnSucess(content);
        }

        private async Task<DownloadHtmlResult> ReturnSucess(string content)
        {
            return await ReturnResult(content, true);
        }

        private async Task<DownloadHtmlResult> ReturnError()
        {
            return await ReturnResult(string.Empty, false);
        }

        private async Task<DownloadHtmlResult> ReturnResult(string content, bool isSucess)
        {
            return await Task.FromResult(new DownloadHtmlResult(content, isSucess, document));
        }
    }
}