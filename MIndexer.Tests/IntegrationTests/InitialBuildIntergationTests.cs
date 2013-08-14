using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MIndexer.Builder;
using MIndexer.Core;
using MIndexer.Core.DataTypes;
using MIndexer.Core.IndexMaintainers;
using MIndexer.Core.Interfaces;
using NUnit.Framework;
using Sciendo.Common.Serialization;

namespace MIndexer.Tests.IntegrationTests
{
    [TestFixture]
    [Ignore]
    [Category("integration")]
    public class InitialBuildIntergationTests
    {
        private string _smallRootFolder;
        private string _smallmapFile;
        private string _bigRootFolder;
        private string _bigmapFile;
        private string _realSmallFolder;
        private string _smallLFolder;
        private List<string> _notDownloadedFiles;

        [SetUp]
        public void SetUp()
        {
            _smallRootFolder = @"Input";
            _smallmapFile = @"smallmap.xml";
            _bigRootFolder = @"C:\Users\octo\Music";
            _bigmapFile = "bigmap.xml";
            _realSmallFolder = @"C:\Users\octo\Music\0-9";
            _smallRootFolder = @"IntegrationTests\Output\SmallFolderLyrics";
            _notDownloadedFiles = new List<string>();
            if(!File.Exists(_smallmapFile))
                File.Delete(_smallmapFile);
        }

        [Test]
        public void BuildMapSmallRepository()
        {
            Program.CreateNewMap(_smallRootFolder, _smallmapFile);
            FileMap _actual = Serializer.DeserializeOneFromFile<FileMap>(_smallmapFile);
            Assert.IsNotNull(_actual);

        }
        [Test]
        public void BuildMapBigRepository()
        {
            Program.CreateNewMap(_bigRootFolder, _bigmapFile);
            FileMap _actual = Serializer.DeserializeOneFromFile<FileMap>(_bigmapFile);
            Assert.IsNotNull(_actual);
        }
        [Test]
        public void UseMapToDownloadAndIndexSmallRepository()
        {
            string realSmallFile = @"realSmallmap.xml";
            Program.CreateNewMap(_realSmallFolder, realSmallFile);
            Program.ProcessMap(_realSmallFolder, realSmallFile, _smallLFolder);
            VerifyDownloadingAndIndexing(realSmallFile, _smallLFolder);
        }

        private void VerifyDownloadingAndIndexing(string fileWithMap, string rootFolder)
        {
            Assert.True(File.Exists(fileWithMap));
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(fileWithMap);
            Assert.IsNotNull(fileMap);
            VerifyFolder(fileMap);
        }

        private void VerifyFolder(FileMap fileMap)
        {
            foreach (FileMap fMap in fileMap.SubFiles.Where(f => !f.FileData.IsFolder))
            {
                if (string.IsNullOrEmpty(fMap.FileData.LName))
                {
                    _notDownloadedFiles.Add(fMap.FileData.Name);
                }
                else
                {
                    Assert.True(File.Exists(fMap.FileData.LName));
                    Assert.AreNotEqual(State.NotTouched, fMap.FileData.LState);
                }
                Assert.AreEqual(State.Indexed, fMap.FileData.State);
                VerifyIndexForLFile(fMap.FileData.LName);
                VerifyIndexForMFile(fMap.FileData.Name);
            }
            foreach (FileMap fldMap in fileMap.SubFiles.Where(f => f.FileData.IsFolder))
            {
                VerifyFolder(fldMap);
            }
        }

        private void VerifyIndexForMFile(string name)
        {
            IContentProvider tagReaderHelper = new MContentProvider();
            IIndexManager indexManager = new MIndexManager(tagReaderHelper);
            string query = FindSuitableWord(tagReaderHelper.GetContentAndTarget(name).Content);
            var results = indexManager.Search(query, 40, new string[] { "tagged" });
            Assert.IsNotNull(results);
            Assert.True(results.Any(r => r == name));

        }

        private void VerifyIndexForLFile(string lName)
        {
            ContentAndTarget lyrics = Serializer.DeserializeOneFromFile<ContentAndTarget>(lName);
            Assert.IsNotNull(lyrics);
            Assert.IsNotNull(lyrics.Content);
            Assert.IsNotNull(lyrics.TargetFileName);
            IContentProvider contentProvider= new LContentProviderLocal();
            IIndexManager indexManager = new LIndexManager(contentProvider);
            string query = FindSuitableWord(lyrics.Content);
            var results = indexManager.Search(query, 40, new string[] { "tagged" });
            Assert.IsNotNull(results);
            Assert.True(results.Any(r => r == lyrics.TargetFileName));
        }

        private string FindSuitableWord(string content)
        {
            throw new NotImplementedException();
        }

        [Test]
        public void UseMapToDownloadAndIndexBigRepository()
        {
            Assert.Fail();
        }

    }
}
