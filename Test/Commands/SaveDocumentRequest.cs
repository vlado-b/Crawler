using System;

namespace Test.Commands
{
    public class SaveDocumentRequest
    {
        public SaveDocumentRequest(Uri uri, byte[] bytes)
        {
            Uri = uri;
            Bytes = bytes;
        }

        public Uri Uri { get; private set; }

        public byte[] Bytes { get; private set; }
    }
}