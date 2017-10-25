using System.IO;
using System.Threading.Tasks;

namespace Crawler.Commands
{
    public class LocalDiskFileSaver : FileSaver
    {
        public async Task SaveFile(string path, byte[] bytes)
        {
            using(var stream = new FileStream(path, FileMode.CreateNew))
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public void CreateFolderIfDosntExist(string path)
        {
            var dirName = Path.GetDirectoryName(path);
            var di = new DirectoryInfo(dirName);
            if (!di.Exists)
                di.Create();
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}