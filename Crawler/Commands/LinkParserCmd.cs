using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler.Commands
{
    public class LinkParserCmd : Command
    {
        private readonly string htmlContent;
        private static readonly string XPATHSelectedForImagesInHtml = "//img[@src]";
        private readonly Uri rootUri;
        private static readonly string XpathSelectorForLinksInHtml = "//a[@href]";

        public LinkParserCmd(string htmlContent, Uri rootUri)
        {
            this.htmlContent = htmlContent;
            this.rootUri = rootUri;
        }


        public IEnumerable<CrawlDocument> ParseLinks()
        {
            var htmlDocument = new HtmlDocument();


            htmlDocument.LoadHtml(htmlContent);
            var result = new List<CrawlDocument>();

            result.AddRange( ExtractImages(htmlDocument) );
            result.AddRange( ExtractLinks(htmlDocument));

            return result;
             
        }

        private IEnumerable<CrawlDocument> ExtractImages(HtmlDocument htmlDocument)
        {
            var validImages = new List<CrawlDocument>();
            var imagesNodes = htmlDocument.DocumentNode.SelectNodes(XPATHSelectedForImagesInHtml);
            if (imagesNodes != null)
            {
                validImages = Enumerable.Distinct<string>(imagesNodes.Select(n => n.Attributes["src"].Value))
                    .Where(u => CanCreateUri(u))
                    .Select(u => CreateAbsoluteUri(u))
                    .Where(u => IsInSameDomain(u))
                    .Where( u=> IsUrlValidForHTTPProtocols(u))
                    .Select(u => GetUriForImage(u)).ToList();
            }

            return validImages;
        }

        private IEnumerable<CrawlDocument> ExtractLinks(HtmlDocument htmlDocument)
        {
            var validLinks = new List<CrawlDocument>();
            var linkNodes = htmlDocument.DocumentNode.SelectNodes(XpathSelectorForLinksInHtml);
            if (linkNodes != null)
            {
                validLinks = Enumerable.Distinct<string>(linkNodes.Select(n => n.Attributes["href"].Value))
                    .Where(u => CanCreateUri(u))
                    .Select(u => CreateAbsoluteUri(u))
                    .Where(u => IsInSameDomain(u))
                    .Where(u => IsUrlValidForHTTPProtocols(u))
                    .Select(u => GetUriForLink(u)).ToList();
            }

            return validLinks;
        }

        private CrawlDocument GetUriForLink(Uri uri)
        {
            return new CrawlDocument(uri, false);
        }

        private bool IsInSameDomain(Uri uri)
        {
            return uri.Host == rootUri.Host;
        }

        private Uri CreateAbsoluteUri(string rawUriFromImageSrc)
        {
            return Uri.IsWellFormedUriString(rawUriFromImageSrc, UriKind.Absolute) ? 
                new Uri(rawUriFromImageSrc, UriKind.Absolute) : new Uri(rootUri, rawUriFromImageSrc);
        }

        private bool CanCreateUri(string rawUrl)
        {
            if( Uri.IsWellFormedUriString(rawUrl, UriKind.Absolute)  )
                return true;
            else
            {
                return CanMakeUriFromRelativeUri(rawUrl);
            }
        }

        private bool CanMakeUriFromRelativeUri(string rawUrl)
        {
            try
            {
                var absUri = new Uri(rootUri, rawUrl);
                var urlIsForValidProtocols = IsUrlValidForHTTPProtocols(absUri);
                return urlIsForValidProtocols;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsUrlValidForHTTPProtocols(Uri absUri)
        {
            return absUri.Scheme.Equals(Uri.UriSchemeHttp) || absUri.Scheme.Equals(Uri.UriSchemeHttps);
        }

        private CrawlDocument GetUriForImage(Uri uri)
        {
            return new CrawlDocument(uri, true);
        }

        public Task<Result> Execute()
        {
            var result = new ParseResult(ParseLinks());
            return Task.FromResult<Result>(result);
        }

    }
}