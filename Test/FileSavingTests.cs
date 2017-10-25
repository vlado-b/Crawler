using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using Test.Commands;
using Test.Properties;

namespace Test
{
    [TestFixture]
    public class FileSavingTests
    {
        private FilePathGenerator filePathGenerator;
        private readonly BytesConverter bytesConverter = new BytesConverter();

        [SetUp]
        public void Setup()
        {
            filePathGenerator = new FilePathGenerator();
        }
        

        [TestCase("http://localhost/minor/major/test.html")]
        [TestCase("http://localhost")]
        public async Task SaveDocumentCommand_saves_file_on_disk(string uri)
        {
            var fileContent = Resources.testHtmlWith2DifferentImagesAnd4Links;
            var bytesForSaving = bytesConverter.GetBytes(fileContent); 

            var uriOfFileFromWeb = new Uri(uri);
            Console.WriteLine($"localpath uri: {uriOfFileFromWeb.LocalPath}");
            var expectedPath = filePathGenerator.GeneratePath(uriOfFileFromWeb.LocalPath);

            
            Console.WriteLine($"AssemblyDirectory {FilePathGenerator.AssemblyDirectory}, expectedPath: {expectedPath}");

            DeleteFileIfExists(expectedPath);

            var request = new SaveDocumentRequest(uriOfFileFromWeb, bytesForSaving);
            var fileSaver = new LocalDiskFileSaver();
            var cmd = new SaveDocumentCmd(request,  fileSaver );
            var result = await cmd.Execute();

            Assert.True( result.IsSucess , "The command should finish with sucess" );
            Assert.True( File.Exists( expectedPath ), "File does not exist!" );
            Assert.AreEqual( bytesForSaving, File.ReadAllBytes(expectedPath) , "The content of the files(bytes) are the same");

        }

        [TestCase(@"c\folder1\folder2\minor\major\test.html", @"c\folder1\folder2", "/minor/major/test.html")]
        [TestCase(@"c\folder1\folder2\minor\major\test.html", @"c\folder1\folder2", "minor/major/test.html")]
        [TestCase(@"c\folder1\folder2\minor\major.html", @"c\folder1\folder2", "minor/major/")]
        [TestCase(@"c\folder1\folder2\minor\major.html", @"c\folder1\folder2", "minor/major")]
        [TestCase(@"c\folder1\folder2\index.html", @"c\folder1\folder2", "")]
        [TestCase(@"c\folder1\folder2\index.html", @"c\folder1\folder2", "/")]
        public void Can_generate_valid_path_for_file(string expected, string rootDirectory, string path)
        {
            Console.WriteLine( $"path: {path} filename: {Path.GetFileName(path)}" );
            Assert.AreEqual(expected, filePathGenerator.GeneratePath(rootDirectory, path));
        }

        private void DeleteFileIfExists(string filePath)
        {
            if( File.Exists(filePath) )
                File.Delete(filePath);
        }

        [Test]
        public void Temp()
        {
            Console.WriteLine( "1" +   new Uri("http://www.google.com/a").LocalPath );
            Console.WriteLine( "2" +  new Uri("http://www.google.com/a/").LocalPath);
        }
    }
}