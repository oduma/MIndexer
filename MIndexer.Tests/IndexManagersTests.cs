using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ID3Sharp;
using MIndexer.Core;
using MIndexer.Core.IndexMaintainers;
using MIndexer.Core.IndexSearchers;
using MIndexer.Core.Interfaces;
using NUnit.Framework;

namespace MIndexer.Tests
{
    [TestFixture]
    public class IndexManagersTests
    {
        private MIndexManager _mIndexManager;
        private LIndexManager _lIndexManager;

        [TearDown]
        public void TearDown()
        {
            if(_mIndexManager!=null)
                _mIndexManager.CloseWriter();
            if(_lIndexManager!=null)
                _lIndexManager.CloseWriter();
        }

        [Test]
        public void MIndexManagerPrepareDocumentOk()
        {
            ITagReaderHelper tagReaderHelper =
                TestHelper.GetMockTagReader(new Dictionary<string, ID3Tag>
                                                {
                                                    {
                                                        @"Input\RootFolder\FileInRoot.mp3",
                                                        new ID3v2Tag
                                                            {
                                                                Artist = "AC/DC",
                                                                Album = "Back In Black",
                                                                Title = "Hells Bells",
                                                                Genre = "Hard Rock"
                                                            }
                                                        }
                                                });
            _mIndexManager = new MIndexManager( tagReaderHelper);
            var document = _mIndexManager.PrepareDocument(@"Input\RootFolder\FileInRoot.mp3");
            Assert.IsNotNull(document);
            Assert.AreEqual("AC/DC Back In Black Hells Bells Hard Rock", document.GetField("tagged").StringValue);
            Assert.AreEqual(@"Input\RootFolder\FileInRoot.mp3", document.GetField("targetfilename").StringValue);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MIndexManagerPrepareDocument_NoFilePath()
        {
            _mIndexManager = new MIndexManager(new TagReaderHelper());
            _mIndexManager.PrepareDocument(null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MIndexManagerPrepareDocumentFileNotFound()
        {
            _mIndexManager = new MIndexManager( new TagReaderHelper());
            _mIndexManager.PrepareDocument("nonexsitent.file");
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MIndexManagerPrepareDocumentWrongType()
        {
            _mIndexManager = new MIndexManager(new TagReaderHelper());
            _mIndexManager.PrepareDocument(@"Data\wrongfiletype.txt");

        }

        [Test]
        public void LIndexManagerPrepareDocumentOk()
        {
            _lIndexManager = new LIndexManager();
            var document = _lIndexManager.PrepareDocument(@"Lyrics\01-Hells Bells.xml");
            Assert.IsNotNull(document);
            Assert.AreEqual(@"I am not sure about the lyrics but I have a clue", document.GetField("tagged").StringValue);
            Assert.AreEqual(@"Data\01-Hells Bells.mp3", document.GetField("targetfilename").StringValue);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LIndexManagerPrepareDocument_NoFilePath()
        {
            _lIndexManager = new LIndexManager();
            _lIndexManager.PrepareDocument(null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LIndexManagerPrepareDocumentFileNotFound()
        {
            _lIndexManager = new LIndexManager();
            _lIndexManager.PrepareDocument("nonexistent.file");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LIndexManagerPrepareDocumentContentNotFound()
        {
            _lIndexManager = new LIndexManager();
            _lIndexManager.PrepareDocument(@"Lyrics\incompleteNoContent.lyrics");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LIndexManagerPrepareDocumentTargetFileNotFound()
        {
            _lIndexManager = new LIndexManager();
            _lIndexManager.PrepareDocument(@"Lyrics\incompleteNoTargetFile.lyrics");
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LIndexManagerPrepareDocumentTagretFileNotExists()
        {
            _lIndexManager = new LIndexManager();
            _lIndexManager.PrepareDocument(@"Lyrics\completeNoTargetFile.lyrics");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void lIndexManagerIndexAFileNotExists()
        {
            _lIndexManager= new LIndexManager();
            _lIndexManager.IndexAFile(@"nonexistent");
        }

        [Test]
        public void LlIndexManagerIndexAFileOk()
        {
            _lIndexManager = new LIndexManager();
            _lIndexManager.IndexAFile(@"Data\01-Hells Bells.mp3");
            var results = _lIndexManager.Search("Hel*", 40, new string[] { "tagged" }).ToList();
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.True(results.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\01-Hells Bells.mp3")));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LlIndexManagerIndexAFileWrongManagerForTheFile()
        {
            _lIndexManager = new LIndexManager();
            _lIndexManager.IndexAFile(@"Data\01-Hells Bells.mp3");
        }
        [Test]
        public void MlIndexManagerIndexAFileOk()
        {
            ITagReaderHelper tagReaderHelper=TestHelper.GetMockTagReader(new Dictionary<string, ID3Tag>
                                                {
                                                    {
                                                        @"Input\RootFolder\FileInRoot.mp3",
                                                        new ID3v2Tag
                                                            {
                                                                Artist = "AC/DC",
                                                                Album = "Back In Black",
                                                                Title = "Hells Bells",
                                                                Genre = "Hard Rock"
                                                            }
                                                        }
                                                });
            _mIndexManager = new MIndexManager(tagReaderHelper);
            _mIndexManager.IndexAFile(@"Input\RootFolder\FileInRoot.mp3");
            _mIndexManager.CommitAndOptimize();
            var results = _mIndexManager.Search("Hel*", 40, new string[] { "tagged" }).ToList();
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.True(results.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Input\RootFolder\FileInRoot.mp3")));
        }

        [Test]
        public void MlIndexManagerIndexAFileWrongManagerForTheFile()
        {
            _lIndexManager = new LIndexManager();
            _lIndexManager.IndexAFile(@"nonexistent");
        }
    }
}
