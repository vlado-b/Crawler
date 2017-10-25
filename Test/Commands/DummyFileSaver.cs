using System;
using System.IO;
using System.Threading.Tasks;

namespace Test.Commands
{
    public class DummyFileSaver : FileSaver
    {
        public async Task SaveFile(string path, byte[] bytes)
        {
            Console.WriteLine($"Saving file {path} !");
        }

        public void CreateFolderIfDosntExist(string path)
        {
        }

        public bool FileExists(string path)
        {
            return false;
        }
    }

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