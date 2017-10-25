using System;
using System.Threading.Tasks;
using Crawler;
using Crawler.Commands;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class DownloadCommandsTests
    {
        private static readonly Uri NonExistingUri = new Uri(  "http://someNonnExistingDomain/SomeNonExistingResource");
        private static readonly Uri URITretton = new Uri("https://tretton37.com/");
        private static readonly Uri Tretton37LogoRemotePath = new Uri("https://tretton37.com/assets/i/main.jpg");
        private readonly CommandAsserts commandAsserts = new CommandAsserts();

        [Test]
        public async Task  Donwload_of_HTML_files_that_have_Non_200_Http_status_for_results_with_error_response()
        {
            var downloadCmdForNonExistingURI = 
                new DownloadHtmlDocumentCmd( new CrawlDocument( NonExistingUri , false )  );

            var result = await downloadCmdForNonExistingURI.Execute();

            commandAsserts.AssertThatCommandFailed(result);
        }


        [Test]
        public async Task Can_download_HTML_file()  
        {
            
            var coommand = new DownloadHtmlDocumentCmd(new CrawlDocument( URITretton, false));

            var result = await coommand.Execute();

            commandAsserts.AssertCommandEndedWithSucess(result);
            commandAsserts.AssertCorrectInstanceTypeOfResult<DownloadHtmlResult>(result);
            Assert.True( ((DownloadHtmlResult)result).Content.ToLowerInvariant().Contains("</html>"));
        }




        [Test]
        public async Task Donwload_of_Image_files_that_have_Non_200_Http_status_for_results_with_error_response()
        {
            var downloadCmdForNonExistingURI =
                new DownloadImageDocumentCmd(new CrawlDocument(NonExistingUri, true));

            var result = await downloadCmdForNonExistingURI.Execute();

            commandAsserts.AssertThatCommandFailed(result);
        }

        [Test]
        public async Task Can_download_Image()
        {

            var coommand = new DownloadImageDocumentCmd(new CrawlDocument(Tretton37LogoRemotePath, false));
            var result = await coommand.Execute();

            commandAsserts.AssertCommandEndedWithSucess(result);
            commandAsserts.AssertCorrectInstanceTypeOfResult<DownloadImageResult>(result);
            Assert.Greater( (result as DownloadImageResult ).Bytes.Length, 0 );
        }
    }


   
}