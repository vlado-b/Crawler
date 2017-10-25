using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Test.Commands
{
    public class DownloadImageDocumentCmd : DownloadDocumentCmd, Command
    {

        public DownloadImageDocumentCmd(CrawlDocument document)
        {
            this.document = document;
        }

        public async Task<Result> Execute()
        {
            var httpClient = new HttpClient();
            byte[] bytes ;

            try
            {
                bytes = await httpClient.GetByteArrayAsync(document.Uri);
            }
            catch (Exception exe )
            {
                Console.WriteLine($"exception {exe.Message} when downloading {document.Uri}" );
                return await ReturnError();
            }

            return await ReturnSucess(bytes );
        }

        private async Task<DownloadImageResult> ReturnSucess(byte[] bytes)
        {
            return await ReturnResult(bytes, true);
        }

        private async Task<DownloadImageResult> ReturnError()
        {
            return await ReturnResult(null, false);
        }

        private async Task<DownloadImageResult> ReturnResult(byte[] bytes, bool isSucess)
        {
            return await Task.FromResult<DownloadImageResult>(new DownloadImageResult( bytes, isSucess, document));
        }
    }
}