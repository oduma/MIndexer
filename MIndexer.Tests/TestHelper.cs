using System.Collections.Generic;
using System.Linq;
using ID3Sharp;
using MIndexer.Core.DataTypes;
using MIndexer.Core.IndexMaintainers;
using MIndexer.Core.Interfaces;
using Moq;
using NUnit.Framework;

namespace MIndexer.Tests
{
    public static class TestHelper
    {
        public static void ValidateFiles(string[] expectedFiles, List<FileMap> actualFiles, bool isLFile = false,State state=State.NotTouched)
        {
            Assert.AreEqual(expectedFiles.Length, actualFiles.Count);
            Assert.False(actualFiles.Any(f => f.FileData.IsFolder));
            if (!isLFile)
            {
                Assert.False(actualFiles.Any(s => s.FileData.State != state));
                Assert.AreEqual(expectedFiles.Length, actualFiles.Count(f => expectedFiles.Contains(f.FileData.Name)));
            }
            else
            {
                Assert.False(actualFiles.Any(s => s.FileData.LState != State.Indexed));
                Assert.AreEqual(expectedFiles.Length, actualFiles.Count(f => expectedFiles.Contains(f.FileData.LName)));
            }
        }

        public static void ValidateFolder(string inputFolder, FileData fileData)
        {
            Assert.True(fileData.IsFolder);
            Assert.AreEqual(State.NotTouched, fileData.State);
            Assert.AreEqual(inputFolder, fileData.Name);
        }

        public static IIndexManager GetMockIndexMaintainer(Dictionary<string,bool> inOuts)
        {
            Mock<IIndexManager> indexMaintainer = new Mock<IIndexManager>(MockBehavior.Default);
            indexMaintainer.CallBase = false;
            indexMaintainer.Setup(m => m.CommitAndOptimize());
            foreach (string key in inOuts.Keys)
            {
                string key1 = key;
                bool val1 = inOuts[key1];
                indexMaintainer.Setup(m => m.IndexAFile(key1)).Returns(val1);
            }
            return indexMaintainer.Object;
        }


        public static ITagReaderHelper GetMockTagReader(Dictionary<string, ID3Tag> inOuts)
        {
            Mock<ITagReaderHelper> tagReaderHelper = new Mock<ITagReaderHelper>(MockBehavior.Default);
            tagReaderHelper.CallBase = false;
            foreach (string key in inOuts.Keys)
            {
                string key1 = key;
                var val1 = inOuts[key1];
                tagReaderHelper.Setup(m => m.GetID3Tag(key1)).Returns(val1);
            }
            return tagReaderHelper.Object;

        }



    }
}
