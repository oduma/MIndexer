using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ID3Sharp;
using MIndexer.Core;
using MIndexer.Core.DataTypes;
using MIndexer.Core.IndexMaintainers;
using MIndexer.Core.Interfaces;
using Moq;
using NUnit.Framework;
using Sciendo.Common.Serialization;

namespace MIndexer.Tests
{
    [TestFixture]
    public class FileMapTests
    {
        private int _numberOfFiles;

        [SetUp]
        public void SetUp()
        {
            _numberOfFiles = 0;
        }

        [Test]
        public void FileMapBuilderStartWithFolderMFileMapOk()
        {
            FileMapBuilder fileMapBuilder = new FileMapBuilder();
            var actual = fileMapBuilder.StartFromFolder(@"Input\RootFolder",IOHelper.GetMExtentions());
            Assert.IsNotNull(actual);
            TestHelper.ValidateFolder(@"Input\RootFolder", actual.FileData);
            TestHelper.ValidateFiles(
            new string[] {
                @"Input\RootFolder\FileInRoot.mp3"
            },actual.SubFiles.Where(f=>!f.FileData.IsFolder).ToList());
            TestHelper.ValidateFolder(@"Input\RootFolder\Folder1",actual.SubFiles.First(s=>s.FileData.IsFolder).FileData);
            TestHelper.ValidateFiles(
            new string[] {
                @"Input\RootFolder\Folder1\File1InFolder1.ogg",
                @"Input\RootFolder\Folder1\File2InFolder1.mp3"
            }, actual.SubFiles.First(s => s.FileData.IsFolder).SubFiles.Where(f => !f.FileData.IsFolder).ToList());
            TestHelper.ValidateFolder(@"Input\RootFolder\Folder1\Folder2",
                           actual.SubFiles.First(s => s.FileData.IsFolder).SubFiles.First(f => f.FileData.IsFolder).
                               FileData);
            TestHelper.ValidateFiles(
            new string[] {
                @"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3",
                @"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac"
            }, actual.SubFiles.First(s => s.FileData.IsFolder).SubFiles.First(f => f.FileData.IsFolder).
                               SubFiles.Where(e => !e.FileData.IsFolder).ToList());
            //Serializer.SerializeOneToFile(actual,@"Input\map.xml");

        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileMapBuilderStartWithFolderNothingSent()
        {
            FileMapBuilder fileMapBuilder = new FileMapBuilder();
            fileMapBuilder.StartFromFolder(null,null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void FileMapBuilderStartWithFolderDoesNotExist()
        {
            FileMapBuilder fileMapBuilder= new FileMapBuilder();
            fileMapBuilder.StartFromFolder(@"Input\WrongFolder",new string[]{"abc"});
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileMapBuilderStartWithFolderNoFilterSent()
        {
            FileMapBuilder fileMapBuilder = new FileMapBuilder();
            fileMapBuilder.StartFromFolder(@"Input\WrongFolder", null);
        }

        [Test]
        public void FileMapGetAllFoldersRecursiveOk()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            var actual = fileMap.GetAllFolders(true);
            TestHelper.ValidateFolder(@"Input\RootFolder", actual[0]);
            TestHelper.ValidateFolder(@"Input\RootFolder\Folder1", actual[1]);
            TestHelper.ValidateFolder(@"Input\RootFolder\Folder1\Folder2", actual[2]);
        }

        [Test]
        public void FileMapGetAllFoldersNonRecursiveOk()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            var actual = fileMap.GetAllFolders(false);
            TestHelper.ValidateFolder(@"Input\RootFolder", actual[0]);
        }
        [Test]
        public void FileMapGetAllFoldersNoFolders()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            FileMap subFileMap = fileMap.SubFiles.First(f => !f.FileData.IsFolder);
            var actual = subFileMap.GetAllFolders(true);
            Assert.False(actual.Any());
        }

        [Test]
        public void FileMapGetSubFileMapFolder()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            FileMap actual = fileMap.GetSubFileMap(@"Input\RootFolder\Folder1\Folder2");
            TestHelper.ValidateFolder(@"Input\RootFolder\Folder1\Folder2", actual.FileData);
            TestHelper.ValidateFiles(
new string[] {
                @"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3",
                @"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac"
            }, actual.SubFiles);

        }

        [Test]
        public void FileMapGetSubFileMapFile()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            FileMap actual = fileMap.GetSubFileMap(@"Input\RootFolder\Folder1\File1InFolder1.ogg");
            Assert.AreEqual(@"Input\RootFolder\Folder1\File1InFolder1.ogg", actual.FileData.Name);
            Assert.False(actual.FileData.IsFolder);
        }
        [Test]
        public void FileMapGetSubFileMapNotFound()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            FileMap actual = fileMap.GetSubFileMap(@"Input\RootFolder\Folder1\Inexistent");
            Assert.IsNull(actual);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileMapGetSubFileMapNothingSent()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            fileMap.GetSubFileMap(null);
        }
        #region Heavy duty tests
        [Test]
        [Ignore]
        public void FileMapBuilderStartWithFolderPerformance()
        {
            Stopwatch stopwatch= new Stopwatch();
            FileMapBuilder fileMapBuilder = new FileMapBuilder();
            fileMapBuilder.FileMapBuildProgress += new EventHandler<FileMapBuildProgressEventArgs>(fileMapBuilder_FileMapBuildProgress);
            stopwatch.Start();
            var actual = fileMapBuilder.StartFromFolder(@"c:\users\octo\music", IOHelper.GetMExtentions());
            Assert.IsNotNull(actual);
            stopwatch.Stop();
            Console.WriteLine("{0} processed in {1}",_numberOfFiles,stopwatch.Elapsed.TotalMilliseconds);
        }

        void fileMapBuilder_FileMapBuildProgress(object sender, FileMapBuildProgressEventArgs e)
        {
            _numberOfFiles += e.Files.Count;
        }
        #endregion

    }
}
