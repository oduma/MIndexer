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
            IContentProvider contentProvider = TestHelper.GetMockContentProvider(new Dictionary<string,ContentAndTarget>
                                      {
                                          {"Input\\RootFolder\\FileInRoot.mp3", new ContentAndTarget{Content= "lyrics1",TargetFileName= "Input\\RootFolder\\FileInRoot.mp3"}},
                                          {@"Input\RootFolder\Folder1\File1InFolder1.ogg", new ContentAndTarget{Content= "lyrics2",TargetFileName= @"Input\RootFolder\Folder1\File1InFolder1.ogg"}},
                                          {@"Input\RootFolder\Folder1\File2InFolder1.mp3", new ContentAndTarget{Content= "lyrics3",TargetFileName= @"Input\RootFolder\Folder1\File2InFolder1.mp3"}},
                                          {@"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3", new ContentAndTarget{Content= "lyrics4",TargetFileName= @"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3"}},
                                          {@"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac", new ContentAndTarget{Content= "lyrics5",TargetFileName= @"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac"}}
                                      });
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
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
            DownloadManager downloadManager= new DownloadManager(@"Input\RootFolder",@"Lyrics",new string[]{"lyrics"},contentProvider,fileMap);
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
            BulkFileProcessor.ProcessLFilesFromMap(new FileMap(), new DownloadManager("Input", "Lyrics", new string[] { "abc" }, new MContentProvider(), new FileMap()), null);
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
