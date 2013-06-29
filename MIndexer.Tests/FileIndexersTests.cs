﻿using System;
using MIndexer.Core;
using NUnit.Framework;

namespace MIndexer.Tests
{
    [TestFixture]
    public class FileIndexersTests
    {
        [Test]
        public void MFile_PrepareDocumentOk()
        {
            MFileIndexerSearcher mFileIndexer = new MFileIndexerSearcher();
            var document = mFileIndexer.PrepareDocument(@"Data\01-Hells Bells.mp3");
            Assert.IsNotNull(document);
            Assert.AreEqual("AC/DC Back In Black Hells Bells Hard Rock", document.GetField("tagged").StringValue);
            Assert.AreEqual(@"Data\01-Hells Bells.mp3",document.GetField("filename").StringValue);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MFile_PrepareDocument_NoFilePath()
        {
            MFileIndexerSearcher mFileIndexer= new MFileIndexerSearcher();
            mFileIndexer.PrepareDocument(null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MFile_PrepareDocumentFileNotFound()
        {
            MFileIndexerSearcher mFileIndexer = new MFileIndexerSearcher();
            mFileIndexer.PrepareDocument("nonexsitent.file");
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MFile_PrepareDocumentWrongType()
        {
            MFileIndexerSearcher mFileIndexer = new MFileIndexerSearcher();
            mFileIndexer.PrepareDocument(@"Data\wrongfiletype.txt");
            
        }
        [Test]
        public void LFile_PrepareDocumentOk()
        {
            Assert.Fail();
        }
        [Test]
        public void LFile_PrepareDocument_NoFilePath()
        {
            Assert.Fail();
        }
        [Test]
        public void LFile_PrepareDocumentFileNotFound()
        {
            Assert.Fail();
        }
        [Test]
        public void LFile_PrepareDocument_NoTargetFilePath()
        {
            Assert.Fail();
        }
        [Test]
        public void LFile_PrepareDocumentTargetFileNotFound()
        {
            Assert.Fail();
        }

    }
}
