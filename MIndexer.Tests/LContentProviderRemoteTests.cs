using System;
using MIndexer.Core;
using MIndexer.Core.DataTypes;
using MIndexer.Core.Interfaces;
using NUnit.Framework;

namespace MIndexer.Tests
{
    [TestFixture]
    public class LContentProviderRemoteTests
    {

        [TestCase(new object[]{null},ExpectedResult=null)]
        [TestCase(new object[] { "" }, ExpectedResult = null)]
        [TestCase(new object[] { "file.mp3" }, ExpectedResult = null)]
        [TestCase(new object[] { @"Lyrics\completeNoTargetFile.lyrics" }, ExpectedResult = null)]
        [TestCase(new object[] { @"Input\RootFolder\FileInRoot.mp3" }, ExpectedResult = null)]
        [TestCase(new object[] { @"Data\Dance 1.mp3" }, ExpectedResult = null)]
        [TestCase(new object[] { @"Data\01-Hells Bells.mp3" }, ExpectedResult = null)]
        public void GetContentAndTargetFileAndConnectionProblems(string filePath)
        {
            var lcontentProvider = new LContentProviderRemote();
            lcontentProvider.GetContentAndTarget(filePath);
        }
    }
}
