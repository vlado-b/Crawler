using System;

namespace Test
{
    public class CrawlDocument //: IEquatable<CrawlDocument>
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