using System;
using MIndexer.Core;
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
            DownloaderHelper downloaderHelper= new DownloaderHelper();
            downloaderHelper.DownloadLyrics(null);
        }
        [Test]
        public void DownloadLyrics_BadUrl()
        {
            DownloaderHelper downloaderHelper = new DownloaderHelper();
            Assert.IsNullOrEmpty(downloaderHelper.DownloadLyrics("crazy url"));
        }
        [Test]
        public void DownloadLyrics_OK()
        {
            DownloaderHelper downloaderHelper = new DownloaderHelper();
            Assert.IsNotNullOrEmpty(downloaderHelper.DownloadLyrics("paloma-faith/new-york-lyrics/"));
        }

    }
}
