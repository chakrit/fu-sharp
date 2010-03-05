
using System;
using System.IO;

using MbUnit.Framework;

using Fu.Services;

namespace Fu.Tests
{
    // TODO: Since this is a sensitive algorithm whose input may came from outside
    //       of the system we should add lots and lots more test cases to ensure
    //       that this thing is properly secured against accidental path traversal.
    //         ex. Malformed path tests, unicode character in paths... localization etc.
    [TestFixture]
    public class PathResolutionTests
    {
        [Test]
        public void BasicResolution()
        {
            // basic path traversal tricks
            resolve(@"C:\Temp", @"C:\Temp",
                null,
                string.Empty,
                @"\", @"/", @"~",
                @".", @"..", @"\..", @"/..",
                @"\css\..", @"/css/..", @"\css/..", @"/css\..",
                @".\..", @"./..");
        }

        [Test]
        public void RelativeBasePath()
        {
            var originalFolder = Environment.CurrentDirectory;

            // create a temp folder tree so we can experiment with traversals
            var temp = Path.GetTempPath();
            temp = Path.Combine(temp, Path.GetTempFileName());

            File.Delete(temp/*as file*/);
            Directory.CreateDirectory(temp/*as folder*/);

            var folderA = Path.Combine(temp, @"a");
            var folderB = Path.Combine(temp, @"a\b");
            var folderC = Path.Combine(temp, @"a\b\c");
            Directory.CreateDirectory(folderA);
            Directory.CreateDirectory(folderB);
            Directory.CreateDirectory(folderC);

            // tests
            Environment.CurrentDirectory = folderC;
            resolve(folderA, @"..\..",
                null, string.Empty, @"\", "/", ".");

            // TODO: Moar cases

            // cleanup
            Environment.CurrentDirectory = originalFolder;
            Directory.Delete(temp, true);
        }

        [Test]
        public void SingleSubfolder()
        {
            // single subfolder test
            resolve(@"C:\Temp\css", @"C:\Temp",
                @"css", @"~css", @"~\css", @"~/css", @"\css", @"/css", @".\css", @"./css",
                @"xxx\..\css", @".\xxx\..\css", @"./xxx/../css");
        }

        [Test]
        public void MultipleSubfolder()
        {
            // multiple subfolder test
            resolve(@"C:\Temp\a\b\c", @"C:\Temp",
                @"a\b\c", @"a/b\c", @"a/b/c", @"./a/b/c", @".\a\b\c",
                @"a\x\..\b\c", @"a\c\..\x\..\b\c");
        }

        [Test]
        public void BasicFilenames()
        {
            // filenames
            resolve(@"C:\Temp\site.css", @"C:\Temp",
                "site.css", "~site.css", "~/site.css");
        }

        [Test]
        public void SingleSubFolderAndFilename()
        {
            // filenames inside single subfolder
            resolve(@"C:\Temp\css\site.css", @"C:\Temp",
                @"css\site.css", "~/css/site.css", "~css/site.css",
                "./css/site.css", @".\css\site.css",
                @"css/../css/site.css", @"css/../css/site.css");
        }

        [Test]
        public void AbsolutePathWithPathProtection()
        {
            resolve(@"C:\Temp", @"C:\Temp",
                @"C:\Temp", @"C:\", @"C:\OtherFolder",
                @"C:\OtherFolder\..\Temp", @"C:\Temp\..\OtherFolder",
                @"C:\Temp\..", @"C:\Temp\..\", @"C:\Temp\.\..\");
        }

        [Test]
        public void AbsolutePathWithoutPathProtection()
        {
            resolve(true, @"C:\Other\a.html", @"C:\Temp",
                @"C:\Other\a.html", @"C:\Temp\..\Other\a.html");
        }

        [Test]
        public void AbsoluteFilenameWithPathProtection()
        {
            resolve(@"C:\Temp\a.html", @"C:\Temp",
                @"C:\Temp\a.html", @"C:\Temp\..\OtherFolder\..\Temp\a.html");
        }

        [Test]
        public void AbsoluteFilenameWithoutPathProtection()
        {
            var basePath = @"C:\Temp";
            var paths = new[] {
                @"C:\Temp\a.html",
                @"C:\Temp\..\a.html",
                @"C:\a.html",
                @"C:\OtherFolder\a.html",
                @"C:\Temp\.\..\OtherFolder\..\.\a.html",
            };

            foreach (var path in paths)
                Assert.AreEqual(Path.GetFullPath(path),
                    PathResolution.ResolvePath(basePath, path, true));
        }


        private void resolve(string expected, string basePath, params string[] testPaths)
        { resolve(false, expected, basePath, testPaths); }

        private void resolve(bool disableProtection, string expected, string basePath, params string[] testPaths)
        {
            foreach (var appPath in testPaths)
            {
                var result = PathResolution.ResolvePath(basePath, appPath, disableProtection);

                Assert.AreEqual(expected, result, string.Format(
                    @"Base: {0} App: {1}", basePath, appPath, result));
            }
        }
    }
}
