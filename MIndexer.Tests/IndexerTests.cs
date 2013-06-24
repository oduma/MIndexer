using System;
using System.IO;
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

        [SetUp]
        public void SetUp()
        {
            _actualFileNames=new List<string>();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuildIndexOnFolder_NoFolder()
        {
            Indexer indexer= new Indexer(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuildIndexOnFolder_NoProcessingMethod()
        {
            Indexer indexer = new Indexer("Data");
            indexer.BuildIndexOnFolder(null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildIndexonFolder_FolderNotExists()
        {
            Indexer indexer = new Indexer("abc");
        }

        private void ProcessTheFile(string fileName, MFileIndexer mFileIndexer,IndexWriter indexWriter)
        {
            _actualFileNames.Add(fileName);
        }

        [Test]
        public void BuildIndexOnFolder_NoProcessing_Ok()
        {
            Indexer indexer = new Indexer("Data");
            indexer.BuildIndexOnFolder(ProcessTheFile);
            Assert.IsNotNull(_actualFileNames);
            Assert.AreEqual(4,_actualFileNames.Count);
            Assert.True(_actualFileNames.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\wronfiletype.txt")));
            Assert.True(_actualFileNames.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Data\01-Hells Bells.mp3")));
            Assert.True(_actualFileNames.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Data\Sub\file1.txt")));
            Assert.True(_actualFileNames.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Data\Sub\SubSub\file2.txt")));
        }
    }
}
