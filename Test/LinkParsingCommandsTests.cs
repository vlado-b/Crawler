using System;
using System.Collections;
using System.Linq;
using Crawler.Commands;
using NUnit.Framework;
using Test.Properties;

namespace Test
{
    [TestFixture]
    public class LinkParsingCommandsTests 
    {
        private LinkParserCmd linkParserCmd;
        private readonly Uri rootUri = new Uri("http://localhost/");
        private readonly CommandAsserts commandAsserts = new CommandAsserts();

        [Test]
        public void When_Empty_html_is_parsed_no_links_are_returned()
        {
            linkParserCmd = CreateLinkParserCommand(string.Empty);
            var result = linkParserCmd.Execute().Result;
            commandAsserts.AssertCommandEndedWithSucess(result);
            commandAsserts.AssertCorrectInstanceTypeOfResult<ParseResult>(result);
            
            Assert.IsEmpty((result as ParseResult).ParsedLinks );
        }

        [Test]
        public void When_Invalid_html_is_parsed_it_will_not_return_any_links()
        {
            var invalidHtml = "some invalid html bla bla ";
            linkParserCmd = CreateLinkParserCommand(invalidHtml);

            var result = linkParserCmd.Execute().Result;
            
            commandAsserts.AssertCommandEndedWithSucess(result);
            commandAsserts.AssertCorrectInstanceTypeOfResult<ParseResult>(result);
            Assert.IsEmpty((result as ParseResult).ParsedLinks);
        }



        [Test] 
        public void Links_are_extracted_from_html()
        {
            var htmlWIth2ImagesAnd4Links = Resources.testHtmlWith2DifferentImagesAnd4Links;
            linkParserCmd = CreateLinkParserCommand(htmlWIth2ImagesAnd4Links);

            var extractedDocuments =  ((ParseResult)linkParserCmd.Execute().Result).ParsedLinks;


            Assert.AreEqual(4, extractedDocuments.Count( d=>! d.IsImage ), "Different number of expected links to be extracted");
            Assert.AreEqual(2, extractedDocuments.Count( d=>d.IsImage),    "Different number of expected images to be extracted");
        }

        private LinkParserCmd CreateLinkParserCommand(string htmlContent)
        {
            return new LinkParserCmd(htmlContent , rootUri );
        }

    }
}
