using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Test.Commands;
using Test.Properties;

namespace Test
{
    [TestFixture]
    public class ParserTests 
    {
        private LinkParserCmd linkParserCmd;
        private Uri rootUri = new Uri("http://localhost/");


        [Test]
        public void Empty_html_is_when_parsed_returns_no_links()
        {
            var emptyHtmlContent = string.Empty;
            linkParserCmd = CreateLinkParser(emptyHtmlContent);
            var result = linkParserCmd.Execute().Result; 
            Assert.IsInstanceOf<ParseResult>(  result );
            Assert.True(  result.IsSucess  );
            Assert.IsEmpty((result as ParseResult).ParsedLinks );
        }

        [Test]
        public void Invalid_html_will_not_return_parse_return_any_links()
        {
            var invalidHtml = "some invalid html bla bla ";
            linkParserCmd = CreateLinkParser(invalidHtml);

            var result = linkParserCmd.Execute().Result;
            Assert.IsInstanceOf<ParseResult>(result);
            Assert.True(result.IsSucess);
            Assert.IsEmpty((result as ParseResult).ParsedLinks);
        }



        [Test] 
        public void Links_are_extracted_from_html()
        {
            var htmlWIth2ImagesAnd4Links = Resources.testHtmlWith2DifferentImagesAnd4Links;
            linkParserCmd = CreateLinkParser(htmlWIth2ImagesAnd4Links);

            var extractedDocuments =  ((ParseResult)linkParserCmd.Execute().Result).ParsedLinks;


            Assert.AreEqual(4, extractedDocuments.Count( d=>! d.IsImage ), "Different number of expected links to be extracted");
            Assert.AreEqual(2, extractedDocuments.Count( d=>d.IsImage),    "Different number of expected images to be extracted");
        }

        private LinkParserCmd CreateLinkParser(string htmlContent)
        {
            return new LinkParserCmd(htmlContent , rootUri );
        }

    }
}
