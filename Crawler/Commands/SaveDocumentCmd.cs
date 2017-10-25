using System;
using System.Threading.Tasks;

namespace Crawler.Commands
{
    public class SaveDocumentCmd : Command
    {
        private readonly SaveDocumentRequest saveDocumentRequest;
        private readonly FileSaver fileSaver;
        private readonly FilePathGenerator filePathGenerator;

        public SaveDocumentCmd(SaveDocumentRequest document, FileSaver fileSaver)
        {
            saveDocumentRequest = document;
            this.fileSaver = fileSaver;
            filePathGenerator = new FilePathGenerator();
        }

        public async Task<Result> Execute()
        {
            string path =  GetPathForFile();

            try
            {
                if( !fileSaver.FileExists(path) )
                {
                    fileSaver.CreateFolderIfDosntExist(path);
                    await fileSaver.SaveFile(path, saveDocumentRequest.Bytes);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception while saving document with URI: {saveDocumentRequest.Uri} to path: {path} error: {e.Message}");
                return await ReturnResult(false);
            }

            return  await ReturnResult(true);

        }

        private string GetPathForFile()
        {
            return filePathGenerator.GeneratePath(saveDocumentRequest.Uri.LocalPath);
        }

        private static async Task<Result> ReturnResult(bool isSucess)
        {
            return await Task.FromResult(new Result(isSucess));
        }
    }
}