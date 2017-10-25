namespace Crawler.Utils
{
    public class BytesConverter
    {
        public byte[] GetBytes(string nonEmptyStringValue)
        {
            byte[] bytes = new byte[nonEmptyStringValue.Length * sizeof(char)];
            System.Buffer.BlockCopy(nonEmptyStringValue.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}