using System;
using System.Threading.Tasks;
using Crawler.Commands;

namespace Test.TestDoubles
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
}