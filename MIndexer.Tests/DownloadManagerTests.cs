using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MIndexer.Core;
using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;
using NUnit.Framework;
using Sciendo.Common.Serialization;

namespace MIndexer.Tests
{
    [TestFixture]
    public class DownloadManagerTests
    {
        private string _dynamicFolder;

        [SetUp]
        public void SetUp()
        {
            _dynamicFolder = Guid.NewGuid().ToString();
            Directory.CreateDirectory(_dynamicFolder);
        }
        [TearDown]
        public void TearDown()
        {
            if(Directory.Exists(_dynamicFolder))
                Directory.Delete(_dynamicFolder,true);
        }
        [TestCase(new object[] { null, null, null, null })]
        [TestCase(new object[] { "Input", null, null, null })]
        [TestCase(new object[] { "Input", @"Lyrics", null, null })]
        [TestCase(new object[] { "Input", @"Lyrics", new string[] { "abc" }, null })]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadManagerConstructorParamsNotSent(string mRootFolder, string lRootFolder, string[] lExtentionsFilter, FileMap fileMap)
        {
            new DownloadManager(mRootFolder, lRootFolder, lExtentionsFilter,
                                                                  new LContentProviderRemote(), fileMap);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadManagerConstructorContentProviderNotSent()
        {
            new DownloadManager("present", "present", new string[] { "abc" },
                                                                  null, new FileMap());
        }

        [TestCase(new object[] { "wrong", "present" })]
        [TestCase(new object[] { "Input", "wrong" })]
        [ExpectedException(typeof(ArgumentException))]
        public void DownloadManagerConstructorSomeWrongParameters(string mRootFolder, string lRootFolder)
        {
            new DownloadManager(mRootFolder, lRootFolder, new string[] { "abc" }, new LContentProviderRemote(), new FileMap());
        }

        [Test]
        public void DownloadFolderOk()
        {
            FileMap fileMap=Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            IContentProvider lcontentProvider = TestHelper.GetMockContentProvider(new Dictionary<string, ContentAndTarget>
                                      {
                                          {"Input\\RootFolder\\FileInRoot.mp3", new ContentAndTarget{Content= "lyrics1",TargetFileName= "Input\\RootFolder\\FileInRoot.mp3"}},
                                          {@"Input\RootFolder\Folder1\File1InFolder1.ogg", new ContentAndTarget{Content= "lyrics2",TargetFileName= @"Input\RootFolder\Folder1\File1InFolder1.ogg"}},
                                          {@"Input\RootFolder\Folder1\File2InFolder1.mp3", new ContentAndTarget{Content= "lyrics3",TargetFileName= @"Input\RootFolder\Folder1\File2InFolder1.mp3"}},
                                          {@"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3", new ContentAndTarget{Content= "lyrics4",TargetFileName= @"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3"}},
                                          {@"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac", new ContentAndTarget{Content= "lyrics5",TargetFileName= @"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac"}}
                                      });
            DownloadManager downloadManager= new DownloadManager(@"Input\RootFolder",@"Lyrics",new string[]{"lyrics"},lcontentProvider,fileMap );
            var actual = downloadManager.DownloadFolder(@"Input\RootFolder\Folder1");

            TestHelper.ValidateFiles(
            new string[] {
                @"Lyrics\Input\RootFolder\Folder1\File1InFolder1.ogg.lyrics",
                @"Lyrics\Input\RootFolder\Folder1\File2InFolder1.mp3.lyrics"
            },actual, true,State.Downloaded);
        }

        [Test]
        public void RetrieveLFileForMWrongFileType()
        {
            DownloadManager downloadManager= new DownloadManager(@"Input\RootFolder",@"Lyrics",new string[]{"lyrics"},new LContentProviderRemote(),new FileMap() );
            Assert.IsNull(downloadManager.RetrieveLyricsForAnMFile(@"Lyrics\01-Hells Bells.xml"));
        }
        [Test]
        public void DownloadFolderLyricsComposedFolderDoesNotExist()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            IContentProvider lcontentProvider = TestHelper.GetMockContentProvider(new Dictionary<string, ContentAndTarget>
                                      {
                                          {"Input\\RootFolder\\FileInRoot.mp3", new ContentAndTarget{Content= "lyrics1",TargetFileName= "Input\\RootFolder\\FileInRoot.mp3"}},
                                          {@"Input\RootFolder\Folder1\File1InFolder1.ogg", new ContentAndTarget{Content= "lyrics2",TargetFileName= @"Input\RootFolder\Folder1\File1InFolder1.ogg"}},
                                          {@"Input\RootFolder\Folder1\File2InFolder1.mp3", new ContentAndTarget{Content= "lyrics3",TargetFileName= @"Input\RootFolder\Folder1\File2InFolder1.mp3"}},
                                          {@"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3", new ContentAndTarget{Content= "lyrics4",TargetFileName= @"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3"}},
                                          {@"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac", new ContentAndTarget{Content= "lyrics5",TargetFileName= @"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac"}}
                                      });
            DownloadManager downloadManager = new DownloadManager(@"Input\RootFolder", _dynamicFolder, new string[] { "lyrics" }, lcontentProvider, fileMap);
            var actual = downloadManager.DownloadFolder(@"Input\RootFolder\Folder1");

            TestHelper.ValidateFiles(
            new string[] {
                _dynamicFolder + @"\Input\RootFolder\Folder1\File1InFolder1.ogg.lyrics",
                _dynamicFolder + @"\Input\RootFolder\Folder1\File2InFolder1.mp3.lyrics"
            }, actual, true, State.Downloaded);
        }
        [Test]
        public void DownloadFolderNoContentForFile()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            IContentProvider lcontentProvider = TestHelper.GetMockContentProvider(new Dictionary<string, ContentAndTarget>
                                      {
                                          {"Input\\RootFolder\\FileInRoot.mp3", null},
                                          {@"Input\RootFolder\Folder1\File1InFolder1.ogg", null},
                                          {@"Input\RootFolder\Folder1\File2InFolder1.mp3", null},
                                          {@"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3", null},
                                          {@"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac", null}
                                      });
            DownloadManager downloadManager = new DownloadManager(@"Input\RootFolder", _dynamicFolder, new string[] { "lyrics" }, lcontentProvider, fileMap);
            Assert.IsNullOrEmpty(downloadManager.RetrieveLyricsForAnMFile(@"Input\RootFolder\Folder1\File1InFolder1.ogg"));
        }
        [Test]
        public void DownloadFolderExceptionDuringContentGet()
        {
            FileMap fileMap = Serializer.DeserializeOneFromFile<FileMap>(@"Input\map.xml");
            IContentProvider lcontentProvider = TestHelper.GetMockContentProviderWithEx(new List<string>
                                      {
                                          {"Input\\RootFolder\\FileInRoot.mp3"},
                                          {@"Input\RootFolder\Folder1\File1InFolder1.ogg"},
                                          {@"Input\RootFolder\Folder1\File2InFolder1.mp3"},
                                          {@"Input\RootFolder\Folder1\Folder2\File1InFolder2.mp3"},
                                          {@"Input\RootFolder\Folder1\Folder2\File2InFolder2.flac"}
                                      });
            DownloadManager downloadManager = new DownloadManager(@"Input\RootFolder", _dynamicFolder, new string[] { "lyrics" }, lcontentProvider, fileMap);
            Assert.IsNullOrEmpty(downloadManager.RetrieveLyricsForAnMFile(@"Input\RootFolder\Folder1\File1InFolder1.ogg"));
        }

    }
}
