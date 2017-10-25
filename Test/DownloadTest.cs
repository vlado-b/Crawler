using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Test.Commands;

namespace Test
{
    public class DownloadTest
    {
        private static readonly Uri NonExistingUri = new Uri(  "http://someNonnExistingDomain/SomeNonExistingResource");
        private static readonly Uri URITretton = new Uri("https://tretton37.com/");
        private static readonly Uri Tretton37LogoRemotePath = new Uri("https://tretton37.com/assets/i/main.jpg");

        [Test]
        public async Task  Non_http_200_status_for_Download_Html_Document_results_with_error_response()
        {
            var downloadCmdForNonExistingURI = 
                new DownloadHtmlDocumentCmd( new CrawlDocument( NonExistingUri , false )  );

            var result = await downloadCmdForNonExistingURI.Execute();

            Assert.False(result.IsSucess , "The command should fail" );
        }

        [Test]
        public async Task Can_download_Html_file()  
        {
            
            var coommand =
                new DownloadHtmlDocumentCmd(new CrawlDocument( URITretton, false));

            var result = await coommand.Execute();

            Assert.True(result.IsSucess, "The command should be sucessfulle");
            Assert.IsInstanceOf<DownloadHtmlResult>(result);
            Assert.True( ((DownloadHtmlResult)result).Content.ToLowerInvariant().Contains("</html>"));
        }


        [Test]
        public async Task Non_http200_status_for_image_download_result_with_error_response()
        {
            var downloadCmdForNonExistingURI =
                new DownloadImageDocumentCmd(new CrawlDocument(NonExistingUri, true));

            var result = await downloadCmdForNonExistingURI.Execute();

            Assert.False(result.IsSucess, "The command should fail");
        }

        [Test]
        public async Task Can_download_image()
        {

            var coommand = new DownloadImageDocumentCmd(new CrawlDocument(Tretton37LogoRemotePath, false));
            var result = await coommand.Execute();
            Assert.True(result.IsSucess, "The command should be sucessfulle");
            Assert.IsInstanceOf<DownloadImageResult>(result);
            Assert.Greater( (result as DownloadImageResult ).Bytes.Length, 0 );
        }



    }


   
}