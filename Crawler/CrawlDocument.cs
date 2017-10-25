using System;

namespace Crawler
{
    public class CrawlDocument
    {
        public CrawlDocument(Uri uri, bool isImage)
        {
            Uri = uri;
            IsImage = isImage;
        }

        public Uri Uri { get; private set; }

        public bool IsImage { get; private set; }


    }

}