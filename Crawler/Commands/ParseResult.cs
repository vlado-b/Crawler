using System.Collections.Generic;
using System.Linq;

namespace Crawler.Commands
{
    public class ParseResult : Result
    {
        public ParseResult(IEnumerable<CrawlDocument> parsedLinks)
        {
            IsSucess = true;
            ParsedLinks = parsedLinks;
        }

        public IEnumerable<CrawlDocument> ParsedLinks { get; }

        public override string ToString()
        {
            return $"ParseResult IsSuccess:{IsSucess}, parsed url links {ParsedLinks.Count(c=>!c.IsImage)} and image {ParsedLinks.Count(c=>c.IsImage)} links";
        }
    }
}