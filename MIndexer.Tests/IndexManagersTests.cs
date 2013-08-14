using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ID3Sharp;
using MIndexer.Core;
using MIndexer.Core.DataTypes;
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
            IContentProvider contentProvider =
                TestHelper.GetMockContentProvider(new Dictionary<string, ContentAndTarget>
                                                {
                                                    {
                                                        @"Input\RootFolder\FileInRoot.mp3",
                                                        new ContentAndTarget
                                                            {
                                                                Content = "AC/DC Back In Black Hells Bells Hard Rock",
                                                                TargetFileName =@"Input\RootFolder\FileInRoot.mp3"
                                                            }
                                                        }
                                                });
            _mIndexManager = new MIndexManager( contentProvider);
            var document = _mIndexManager.PrepareDocument(@"Input\RootFolder\FileInRoot.mp3");
            Assert.IsNotNull(document);
            Assert.AreEqual("AC/DC Back In Black Hells Bells Hard Rock", document.GetField("tagged").StringValue);
            Assert.AreEqual(@"Input\RootFolder\FileInRoot.mp3", document.GetField("targetfilename").StringValue);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MIndexManagerPrepareDocument_NoFilePath()
        {
            _mIndexManager = new MIndexManager(new MContentProvider());
            _mIndexManager.PrepareDocument(null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MIndexManagerPrepareDocumentFileNotFound()
        {
            _mIndexManager = new MIndexManager( new MContentProvider());
            _mIndexManager.PrepareDocument("nonexsitent.file");
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MIndexManagerPrepareDocumentWrongType()
        {
            _mIndexManager = new MIndexManager(new MContentProvider());
            _mIndexManager.PrepareDocument(@"Data\wrongfiletype.txt");

        }

        [Test]
        public void LIndexManagerPrepareDocumentOk()
        {
            _lIndexManager = new LIndexManager(new LContentProviderLocal());
            var document = _lIndexManager.PrepareDocument(@"Lyrics\01-Hells Bells.xml");
            Assert.IsNotNull(document);
            Assert.AreEqual(@"I am not sure about the lyrics but I have a clue", document.GetField("tagged").StringValue);
            Assert.AreEqual(@"Data\01-Hells Bells.mp3", document.GetField("targetfilename").StringValue);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LIndexManagerPrepareDocument_NoFilePath()
        {
            _lIndexManager = new LIndexManager(new LContentProviderLocal());
            _lIndexManager.PrepareDocument(null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LIndexManagerPrepareDocumentFileNotFound()
        {
            _lIndexManager = new LIndexManager(new LContentProviderLocal());
            _lIndexManager.PrepareDocument("nonexistent.file");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LIndexManagerPrepareDocumentContentNotFound()
        {
            _lIndexManager = new LIndexManager(new LContentProviderLocal());
            _lIndexManager.PrepareDocument(@"Lyrics\incompleteNoContent.lyrics");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LIndexManagerPrepareDocumentTargetFileNotFound()
        {
            _lIndexManager = new LIndexManager(new LContentProviderLocal());
            _lIndexManager.PrepareDocument(@"Lyrics\incompleteNoTargetFile.lyrics");
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LIndexManagerPrepareDocumentTagretFileNotExists()
        {
            _lIndexManager = new LIndexManager(new LContentProviderLocal());
            _lIndexManager.PrepareDocument(@"Lyrics\completeNoTargetFile.lyrics");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void lIndexManagerIndexAFileNotExists()
        {
            _lIndexManager= new LIndexManager(new LContentProviderLocal());
            _lIndexManager.IndexAFile(@"nonexistent");
        }

        [Test]
        public void LIndexManagerIndexAFileOk()
        {
            File.Copy(@"Lyrics\01-Hells Bells.xml", @"Lyrics\01-Hells Bells.lyrics", true);

            _lIndexManager = new LIndexManager(new LContentProviderLocal());
            _lIndexManager.IndexAFile(@"Lyrics\01-Hells Bells.lyrics");
            var results = _lIndexManager.Search("about*", 40, new string[] { "tagged" }).ToList();
            _lIndexManager.CommitAndOptimize();
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.True(results.Contains(@"Data\01-Hells Bells.mp3"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LIndexManagerIndexAFileWrongManagerForTheFile()
        {
            _lIndexManager = new LIndexManager(new LContentProviderLocal());
            _lIndexManager.IndexAFile(@"Data\01-Hells Bells.mp3");
        }
        [Test]
        public void MIndexManagerIndexAFileOk()
        {
            IContentProvider contentProvider =
                TestHelper.GetMockContentProvider(new Dictionary<string, ContentAndTarget>
                                                {
                                                    {
                                                        @"Input\RootFolder\FileInRoot.mp3",
                                                        new ContentAndTarget
                                                            {
                                                                Content = "AC/DC Back In Black Hells Bells Hard Rock",
                                                                TargetFileName =@"Input\RootFolder\FileInRoot.mp3"
                                                            }
                                                        }
                                                });
            _mIndexManager = new MIndexManager(contentProvider);
            _mIndexManager.IndexAFile(@"Input\RootFolder\FileInRoot.mp3");
            _mIndexManager.CommitAndOptimize();
            var results = _mIndexManager.Search("Hel*", 40, new string[] { "tagged" }).ToList();
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.True(results.Contains(@"Input\RootFolder\FileInRoot.mp3"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MIndexManagerIndexAFileWrongManagerForTheFile()
        {
            _mIndexManager = new MIndexManager(new MContentProvider());
            _mIndexManager.IndexAFile(@"Lyrics\completeNoTargetFile.lyrics");
        }
    }
}
