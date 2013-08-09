using System;
using MIndexer.Core;
using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;
using NUnit.Framework;

namespace MIndexer.Tests
{
    [TestFixture]
    public class DownloaderTests
    {

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadLyrics_NoUrl()
        {
            DownloadManager DownloadManager = new DownloadManager("Input", @"Lyrics", new string[] { "abc" }, new TagReaderHelper(), new FileMap());
            DownloadManager.DownloadLyrics(null);
        }
        [Test]
        public void DownloadLyrics_BadUrl()
        {
            DownloadManager DownloadManager = new DownloadManager("Input", @"Lyrics", new string[] { "abc" }, new TagReaderHelper(), new FileMap());
            Assert.IsNullOrEmpty(DownloadManager.DownloadLyrics("crazy url"));
        }
        [Test]
        [Ignore("Integration test")]
        public void DownloadLyrics_OK()
        {
            DownloadManager DownloadManager = new DownloadManager("Input", @"Lyrics", new string[] { "abc" }, new TagReaderHelper(), new FileMap());
            Assert.IsNotNullOrEmpty(DownloadManager.DownloadLyrics("paloma-faith/new-york-lyrics/"));
        }

        [TestCase(new object[] { null, null, null,null })]
        [TestCase(new object[] { "Input", null, null, null })]
        [TestCase(new object[] { "Input", @"Lyrics", null, null })]
        [TestCase(new object[] { "Input", @"Lyrics", new string[] { "abc" }, null })]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadManagerConstructorParamsNotSent(string mRootFolder, string lRootFolder, string[] lExtentionsFilter,  FileMap fileMap)
        {
            new DownloadManager(mRootFolder, lRootFolder, lExtentionsFilter,
                                                                  new TagReaderHelper(), fileMap);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DownloadManagerConstructorTagReaderHelperNotSent()
        {
            new DownloadManager("present", "present", new string[]{"abc"}, 
                                                                  null, new FileMap());
        }

        [TestCase(new object[] { "wrong", "present" })]
        [TestCase(new object[] { "Input", "wrong" })]
        [ExpectedException(typeof(ArgumentException))]
        public void DownloadManagerConstructorSomeWrongParameters(string mRootFolder, string lRootFolder)
        {
            new DownloadManager(mRootFolder, lRootFolder, new string[] { "abc" }, new TagReaderHelper(), new FileMap());
        }

    }
}
