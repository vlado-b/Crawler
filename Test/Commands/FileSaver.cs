using System.Threading.Tasks;

namespace Test.Commands
{
    public interface FileSaver
    {
        Task SaveFile(string path, byte[] bytes);

        void CreateFolderIfDosntExist(string path);

        bool FileExists(string path);
    }
}