using System;
using System.IO;
using System.Linq;
using Lucene.Net.Index;
using MIndexer.Core;
using NUnit.Framework;
using System.Collections.Generic;

namespace MIndexer.Tests
{
    [TestFixture]
    public class IndexerTests
    {
        private List<string> _actualFileNames;
        private Indexer _indexer;

        [SetUp]
        public void SetUp()
        {
            _actualFileNames=new List<string>();
        }

        [TearDown]
        public void TearDown()
        {
            if(_indexer!=null)
                _indexer.Dispose();    
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuildIndexOnFolder_NoFolder()
        {
            _indexer= new Indexer(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuildIndexOnFolder_NoProcessingMethod()
        {
            _indexer = new Indexer("Data");
            _indexer.BuildIndexOnFolder(null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildIndexonFolder_FolderNotExists()
        {
            _indexer = new Indexer("abc");
        }

        private void ProcessTheFile(string fileName)
        {
            _actualFileNames.Add(fileName);
        }

        [Test]
        public void BuildIndexOnFolder_NoProcessing_Ok()
        {
            _indexer = new Indexer("Data");
            _indexer.BuildIndexOnFolder(new List<Action<string>> {ProcessTheFile});
            Assert.IsNotNull(_actualFileNames);
            Assert.AreEqual(4,_actualFileNames.Count);
            Assert.True(_actualFileNames.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\wronfiletype.txt")));
            Assert.True(_actualFileNames.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Data\01-Hells Bells.mp3")));
            Assert.True(_actualFileNames.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Data\Sub\file1.txt")));
            Assert.True(_actualFileNames.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Data\Sub\SubSub\file2.txt")));
        }

        [Test]
        public void BuildIndexOnFolder_Processing_OnlyIndexing_Ok()
        {
            _indexer = new Indexer("Data");
            _indexer.BuildIndexOnFolder(new List<Action<string>>{_indexer.IndexAnMFile});

            MFileIndexerSearcher mFileIndexerSearcher= new MFileIndexerSearcher();
            var results = mFileIndexerSearcher.Search("Hel*", 40).ToList();
            Assert.IsNotNull(results);
            Assert.AreEqual(1,results.Count);
            Assert.True(results.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\01-Hells Bells.mp3")));
        }
    }
}
