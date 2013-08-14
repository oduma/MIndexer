using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using MIndexer.Builder;
using NUnit.Framework;

namespace MIndexer.Tests
{
    [TestFixture]
    public class CommandLineOptionsTests
    {
        [TestCase(new object[] { new string[]{"--create=mymap.xml", "--inFolder=myfolder"} },ExpectedResult="mymap.xml")]
        [TestCase(new object[] { new string[] { "-cmymap.xml", "-imyfolder" } }, ExpectedResult = "mymap.xml")]
        [TestCase(new object[] { new string[] { @"--create=my map.xml", "-imyfolder" } }, ExpectedResult = "my map.xml")]
        [TestCase(new object[] { new string[] { @"--create=c:\my folder\my map.xml", "-imyfolder" } }, ExpectedResult = @"c:\my folder\my map.xml")]
        [TestCase(new object[] { new string[] { @"-cc:\my folder\my map.xml", "-imyfolder" } }, ExpectedResult = @"c:\my folder\my map.xml")]
        [TestCase(new object[] { new string[] { "-imyfolder" } }, ExpectedResult = "map.xml")]
        public string OptionsSetForCreateHappyPath(string[] args)
        {
            var options = new Options();
            Parser parser = new Parser(new ParserSettings { MutuallyExclusive = true });
            bool success = parser.ParseArguments(args, options);

            Assert.IsTrue(success);
            Assert.AreEqual("myfolder",options.StartFromFolder);
            return options.CreateNewMap;
        }

        [TestCase(new object[] { new string[] { "--use=mymap.xml", "--inFolder=myfolder" } }, ExpectedResult = "mymap.xml")]
        [TestCase(new object[] { new string[] { "-umymap.xml", "-imyfolder" } }, ExpectedResult = "mymap.xml")]
        [TestCase(new object[] { new string[] { @"--use=my map.xml", "-imyfolder" } }, ExpectedResult = "my map.xml")]
        [TestCase(new object[] { new string[] { @"--use=c:\my folder\my map.xml", "-imyfolder" } }, ExpectedResult = @"c:\my folder\my map.xml")]
        [TestCase(new object[] { new string[] { @"-uc:\my folder\my map.xml", "-imyfolder" } }, ExpectedResult = @"c:\my folder\my map.xml")]
        public string OptionsSetForUseHappyPath(string[] args)
        {
            var options = new Options();
            Parser parser = new Parser(new ParserSettings { MutuallyExclusive = true });
            bool success = parser.ParseArguments(args, options);

            Assert.IsTrue(success);
            Assert.AreEqual("myfolder", options.StartFromFolder);
            return options.UseMap;
        }

        [TestCase(new object[] { new string[] { "create","--create=mymap.xml"} }, ExpectedResult = false)]
        [TestCase(new object[] { new string[] { "-cmymap.xml", "-xmyfolder" } }, ExpectedResult = false)]
        [TestCase(new object[] { new string[] { "-cmymap.xml", "-imyfolder","--use=mymap.xml" } }, ExpectedResult = false)]
        [TestCase(new object[] { new string[] { "-umymap.xml", "-imyfolder", "--create=mymap.xml" } }, ExpectedResult = false)]
        public bool OptionsSetForCreateNegativeTests(string[] args)
        {
            var options = new Options();
            Parser parser = new Parser(new ParserSettings{MutuallyExclusive=true});
            bool success = parser.ParseArguments(args, options);
            if (string.IsNullOrEmpty(options.CreateNewMap) && string.IsNullOrEmpty(options.UseMap))
                success = false;
            return success;
        }

        [TestCase(new object[] { new string[] { "?" } }, ExpectedResult = @"MIndexer.Builder 1.0.0.0

  c, create          (Default: map.xml) Creates a new map file
  u, use             (Default: ) Use a map file
  i, inFolder        Required. (Default: ) Generates a xml file based on data 
                     from the folder structure.
  l, lirycsFolder    (Default: ) Folder where the lyrics are to be stored.
  ?                  Display this help on the screen.
")]
        public string OptionsSetForHelp(string[] args)
        {
            var options = new Options();
            Parser parser = new Parser(new ParserSettings { MutuallyExclusive = true });
            bool success = parser.ParseArguments(args, options);
            Assert.False(success);
            string usage=options.GetUssage();
            return usage;
        }
    }
}
