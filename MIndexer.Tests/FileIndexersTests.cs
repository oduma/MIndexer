using System;
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
            LFileIndexerSearcher lFileIndexer = new LFileIndexerSearcher();
            var document = lFileIndexer.PrepareDocument(@"Lyrics\01-Hells Bells.xml");
            Assert.IsNotNull(document);
            Assert.AreEqual(@"I am not sure about the lyrics but I have a clue", document.GetField("tagged").StringValue);
            Assert.AreEqual(@"Data\01-Hells Bells.mp3", document.GetField("filename").StringValue);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LFile_PrepareDocument_NoFilePath()
        {
            LFileIndexerSearcher lFileIndexer = new LFileIndexerSearcher();
            lFileIndexer.PrepareDocument(null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LFile_PrepareDocumentFileNotFound()
        {
            LFileIndexerSearcher lFileIndexer = new LFileIndexerSearcher();
            lFileIndexer.PrepareDocument("nonexistent.file");
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LFile_PrepareDocument_WrongType()
        {
            LFileIndexerSearcher lFileIndexer = new LFileIndexerSearcher();
            lFileIndexer.PrepareDocument(@"Lyrics\wrongfiletype.lyrics");
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LFile_PrepareDocumentContentNotFound()
        {
            LFileIndexerSearcher lFileIndexer = new LFileIndexerSearcher();
            lFileIndexer.PrepareDocument(@"Lyrics\incompleteNoContent.lyrics");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LFile_PrepareDocumentTargetFileNotFound()
        {
            LFileIndexerSearcher lFileIndexer = new LFileIndexerSearcher();
            lFileIndexer.PrepareDocument(@"Lyrics\incompleteNoTargetFile.lyrics");
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LFile_PrepareDocumentTagretFileNotExists()
        {
            LFileIndexerSearcher lFileIndexer = new LFileIndexerSearcher();
            lFileIndexer.PrepareDocument(@"Lyrics\completeNoTargetFile.lyrics");
        }
    }
}
