using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MIndexer.Core;
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
    }
}
