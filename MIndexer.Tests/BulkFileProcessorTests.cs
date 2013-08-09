using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public class BulkFileProcessorTests
    {
        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists(@"Lyrics\Input"))
                Directory.Delete(@"Lyrics\Input", true);
        }

        [Test]
        public void BulkFileProcessorProcessLFilesFromMapOk()
        {
            ITagReaderHelper tagReaderHelper = TestHelper.GetMockTagReader(new Dictionary<string, ID3Tag>
                                      {
                                          {"Input\\RootFolder\\FileInRoot.mp3", new ID3v2Tag{Artist="a1",Title="t1"}},
                                          {@"Input\RootFolder\Folder1\File1InFolder1.ogg", new ID3v2Tag{Artist="a1",Title="t2"}},
                                          {@"Input\RootFolder\Folder1\File2InFolder1.mp3", new ID3v2Tag{Artist="a1",Title="t3"}},
                                          {@"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3", new ID3v2Tag{Artist="a1",Title="t4"}},
                                          {@"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac", new ID3v2Tag{Artist="a1",Title="t5"}}
                                      });
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            IDownloadManager downloadManager =
                GetMockDownloader(new Dictionary<string, string>
                                      {
                                          {"a1/t1-lyrics/", "lyrics1"},
                                          {"a1/t2-lyrics/", "lyrics2"},
                                          {"a1/t3-lyrics/", "lyrics3"},
                                          {"a1/t4-lyrics/", "lyrics4"},
                                          {"a1/t5-lyrics/", "lyrics5"}
                                      }, "Input", "Lyrics", new string[] { "lyrics" }, tagReaderHelper, fileMap);
            BulkFileProcessor bulkFileProcessor = new BulkFileProcessor();
            IIndexManager lIndexManager = TestHelper.GetMockIndexManager(new Dictionary<string, bool>
                                                                                      {
                                                                                          {
                                                                                              @"Lyrics\Input\RootFolder\FileInRoot.mp3.lyrics"
                                                                                              , true
                                                                                              },
                                                                                          {
                                                                                              @"Lyrics\Input\RootFolder\Folder1\File1InFolder1.ogg.lyrics"
                                                                                              , true
                                                                                              },
                                                                                          {
                                                                                              @"Lyrics\Input\RootFolder\Folder1\File2InFolder1.mp3.lyrics"
                                                                                              , true
                                                                                              },
                                                                                          {
                                                                                              @"Lyrics\Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3.lyrics"
                                                                                              , true
                                                                                              },
                                                                                          {
                                                                                              @"Lyrics\Input\RootFolder\Folder1\Folder2\File2InFolder2.flac.lyrics"
                                                                                              , true
                                                                                              }
                                                                                      });
            bulkFileProcessor.ProcessLFilesFromMap(fileMap, downloadManager, lIndexManager);
            Assert.IsNotNull(fileMap);
            TestHelper.ValidateFiles(
            new string[] {
                @"Lyrics\Input\RootFolder\FileInRoot.mp3.lyrics"
            }, fileMap.SubFiles.Where(f => !f.FileData.IsFolder).ToList(), true);

            TestHelper.ValidateFiles(
            new string[] {
                @"Lyrics\Input\RootFolder\Folder1\File1InFolder1.ogg.lyrics",
                @"Lyrics\Input\RootFolder\Folder1\File2InFolder1.mp3.lyrics"
            }, fileMap.SubFiles.First(s => s.FileData.IsFolder).SubFiles.Where(f => !f.FileData.IsFolder).ToList(), true);
            TestHelper.ValidateFiles(
            new string[] {
                @"Lyrics\Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3.lyrics",
                @"Lyrics\Input\RootFolder\Folder1\Folder2\File2InFolder2.flac.lyrics"
            }, fileMap.SubFiles.First(s => s.FileData.IsFolder).SubFiles.First(f => f.FileData.IsFolder).
                               SubFiles.Where(e => !e.FileData.IsFolder).ToList(), true);

        }

        private IDownloadManager GetMockDownloader(Dictionary<string, string> inOuts, object ctorArg1, object ctorArg2, object ctorArg3, object ctorArg4, object ctorArg5)
        {
            Mock<DownloadManager> mockDownloaderHelper = new Mock<DownloadManager>(ctorArg1, ctorArg2, ctorArg3,
                                                                                     ctorArg4, ctorArg5);
            foreach (string key in inOuts.Keys)
            {
                string key1 = key;
                mockDownloaderHelper.Setup(m => m.DownloadLyrics(key1)).Returns(inOuts[key1]);
            }
            return mockDownloaderHelper.Object;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BulkFileProcessorProcessLFilesFromMapNoMapSent()
        {
            BulkFileProcessor BulkFileProcessor = new BulkFileProcessor();
            BulkFileProcessor.ProcessLFilesFromMap(null, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BulkFileProcessorProcessLFilesFromMapNoDwonloadHelperSent()
        {
            BulkFileProcessor BulkFileProcessor = new BulkFileProcessor();
            BulkFileProcessor.ProcessLFilesFromMap(new FileMap(), null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BulkFileProcessorProcessLFilesFromMapNoIndexMaintainerSent()
        {
            BulkFileProcessor BulkFileProcessor = new BulkFileProcessor();
            BulkFileProcessor.ProcessLFilesFromMap(new FileMap(), new DownloadManager("Input", "Lyrics", new string[] { "abc" }, new TagReaderHelper(), new FileMap()), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BulkFileProcessorProcessMFilesFromMapNoMapSent()
        {
            BulkFileProcessor bulkFileProcessor= new BulkFileProcessor();
            bulkFileProcessor.ProcessMFilesFromMap(null,null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BulkFileProcessorProcessMFilesFromMapNoIndexMaintainerSent()
        {
            BulkFileProcessor bulkFileProcessor = new BulkFileProcessor();
            bulkFileProcessor.ProcessMFilesFromMap(new FileMap(), null);
        }
        [Test]
        public void BulkFileProcessorProcessMFilesFromMapOk()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            IIndexManager mIndexMaintainer = TestHelper.GetMockIndexManager(new Dictionary<string, bool>
                                                                                      {
                                                                                          {
                                                                                              @"Input\RootFolder\FileInRoot.mp3"
                                                                                              , true
                                                                                              },
                                                                                          {
                                                                                              @"Input\RootFolder\Folder1\File1InFolder1.ogg"
                                                                                              , true
                                                                                              },
                                                                                          {
                                                                                              @"Input\RootFolder\Folder1\File2InFolder1.mp3"
                                                                                              , true
                                                                                              },
                                                                                          {
                                                                                              @"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3"
                                                                                              , true
                                                                                              },
                                                                                          {
                                                                                              @"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac"
                                                                                              , true
                                                                                              }
                                                                                      });

            BulkFileProcessor bulkFileProcessor = new BulkFileProcessor();
            bulkFileProcessor.ProcessMFilesFromMap(fileMap, mIndexMaintainer);
            Assert.IsNotNull(fileMap);
            TestHelper.ValidateFiles(
            new string[] {
                @"Input\RootFolder\FileInRoot.mp3"
            }, fileMap.SubFiles.Where(f => !f.FileData.IsFolder).ToList(), false,State.Indexed);

            TestHelper.ValidateFiles(
            new string[] {
                @"Input\RootFolder\Folder1\File1InFolder1.ogg",
                @"Input\RootFolder\Folder1\File2InFolder1.mp3"
            }, fileMap.SubFiles.First(s => s.FileData.IsFolder).SubFiles.Where(f => !f.FileData.IsFolder).ToList(), false,State.Indexed);
            TestHelper.ValidateFiles(
            new string[] {
                @"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3",
                @"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac"
            }, fileMap.SubFiles.First(s => s.FileData.IsFolder).SubFiles.First(f => f.FileData.IsFolder).
                               SubFiles.Where(e => !e.FileData.IsFolder).ToList(), false,State.Indexed);


        }

    }
}
