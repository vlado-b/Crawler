using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Crawler;
using Crawler.Commands;
using Crawler.Utils;
using NUnit.Framework;
using Test.Properties;

namespace Test
{
    [TestFixture]
    public class FileSavingCommandTests
    {
        private readonly FilePathGenerator filePathGenerator;
        private readonly BytesConverter bytesConverter;
        private readonly LocalDiskFileSaver localDiskFileSaver;
        private readonly CommandAsserts commandAsserts;

        public FileSavingCommandTests()
        {
            filePathGenerator = new FilePathGenerator();
            localDiskFileSaver = new LocalDiskFileSaver();
            bytesConverter = new BytesConverter();
            commandAsserts = new CommandAsserts();
        }

    
        [TestCase("http://localhost/minor/major/test.html")]
        [TestCase("http://localhost")]
        public async Task SaveDocumentCommand_saves_file_on_disk(string uri)
        {
            var fileContent = Resources.testHtmlWith2DifferentImagesAnd4Links;
            var bytesForSaving = bytesConverter.GetBytes(fileContent); 

            var uriOfFileFromWeb = new Uri(uri);
            var expectedPath = filePathGenerator.GeneratePath(uriOfFileFromWeb.LocalPath);

            DeleteFileIfExists(expectedPath);

            var request = new SaveDocumentRequest(uriOfFileFromWeb, bytesForSaving);
            var cmd = new SaveDocumentCmd(request,  localDiskFileSaver );
            var result = await cmd.Execute();

            commandAsserts.AssertCommandEndedWithSucess(result);
            Assert.True( File.Exists( expectedPath ), "File does not exist!" );
            Assert.AreEqual( bytesForSaving, File.ReadAllBytes(expectedPath) , "The content of the files(bytes) are the same");

        }

        [TestCase(@"c\folder1\folder2\minor\major\test.html", @"c\folder1\folder2", "/minor/major/test.html")]
        [TestCase(@"c\folder1\folder2\minor\major\test.html", @"c\folder1\folder2", "minor/major/test.html")]
        [TestCase(@"c\folder1\folder2\minor\major.html", @"c\folder1\folder2", "minor/major/")]
        [TestCase(@"c\folder1\folder2\minor\major.html", @"c\folder1\folder2", "minor/major")]
        [TestCase(@"c\folder1\folder2\index.html", @"c\folder1\folder2", "")]
        [TestCase(@"c\folder1\folder2\index.html", @"c\folder1\folder2", "/")]
        public void File_path_and_name_are_generated_properly(string expected, string rootDirectory, string path)
        {
            Assert.AreEqual(expected, filePathGenerator.GeneratePath(rootDirectory, path));
        }

        private void DeleteFileIfExists(string filePath)
        {
            if( File.Exists(filePath) )
                File.Delete(filePath);
        }
 
    }
}