using System.Collections.Generic;
using MIndexer.Core;
using MIndexer.Core.IndexMaintainers;
using NUnit.Framework;

namespace MIndexer.Tests
{
    [TestFixture]
    public class ExtentionMethodsTests
    {
        [Test]
        public void ReplaceAny_Ok()
        {
            Assert.AreEqual("abcdef","a.b,c:d:e,f.".ReplaceAny(new string[] {":",",","."},""));
        }

        [Test]
        public void ReplaceAny_NoReplace()
        {
            Assert.AreEqual("abcdef", "abcdef".ReplaceAny(new string[] { ":", ",", "." }, ""));
        }

        [Test]
        public void ReplaceAny_NoReplaceSent()
        {
            Assert.AreEqual("abcdef", "abcdef".ReplaceAny(null, ""));
        }

        [Test]
        public void ReplaceAny_NoReplaceWidthSent()
        {
            Assert.AreEqual("a.b,c:d:e,f.", "a.b,c:d:e,f.".ReplaceAny(new string[] { ":", ",", "." }, null));
        }

        [Test]
        public void ReplaceAny_Null()
        {
            Assert.IsNullOrEmpty(string.Empty.ReplaceAny(new string[] { ":", ",", "." }, ""));
        }

        [Test]
        public void IndexFileEmptyFilePath()
        {
            IIndexManager indexManager= TestHelper.GetMockIndexManager(new Dictionary<string, bool>{{"abc",true}});
            Assert.False("".IndexFile(indexManager));
        }

        [Test]
        public void IndexFileNoIndexMaintainer()
        {
            Assert.False("somefile".IndexFile(null));
        }
        [Test]
        public void IndexFileFileDoesNotExist()
        {
            IIndexManager indexManager=TestHelper.GetMockIndexManager(new Dictionary<string, bool>{{"abc",true}});
            Assert.False("somefile".IndexFile(indexManager));
        }

        [TestCase(new object[]{@"Input\RootFolder\FileInRoot.mp3",true},ExpectedResult=true)]
        [TestCase(new object[] { @"Input\RootFolder\Folder1\File2InFolder1.mp3", false }, ExpectedResult = false)]
        public bool IndexFile(string someFile, bool valueReturnedByTheIndexMaintainer)
        {
            IIndexManager indexMaintainer=TestHelper.GetMockIndexManager(new Dictionary<string,bool>{{someFile,valueReturnedByTheIndexMaintainer}});
            return someFile.IndexFile(indexMaintainer);
        }
    }
}
