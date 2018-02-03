using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrLfCleaner;

namespace CrLfCleaner.Tests
{
    [TestClass]
    public class ConsoleTests
    {
        [TestMethod]
        public void Console_NoParameters()
        {
            string[] args = { };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(1, container.ReturnCode);
        }

        [TestMethod]
        public void Console_NoFile()
        {
            string[] args =
            {
                "placeholderParam=Hi",
            };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(2, container.ReturnCode);
        }

        [TestMethod]
        public void Console_ValidParams_FileWithoutQuotes()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
                "delimiter=,",
                "hasHeaders=Y",
            };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(@"\\My\File.txt", container.File);
            Assert.AreEqual(',', container.Delimiter);
            Assert.AreEqual(true, container.HasHeaders);
            Assert.AreEqual("", container.Qualifier);
        }

        [TestMethod]
        public void Console_ValidParams_FileWithQuotes()
        {
            string[] args =
            {
                @"file=\\My New\File.txt",
                "delimiter=,",
                "hasHeaders=Y",
            };
            var container = ArgChecker.Check(args);
            Assert.AreEqual("\\\\My New\\File.txt", container.File);
            Assert.AreEqual(',', container.Delimiter);
            Assert.AreEqual(true, container.HasHeaders);
            Assert.AreEqual("", container.Qualifier);
        }

        [TestMethod]
        public void Console_NoDelimiter()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
            };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(4, container.ReturnCode);
        }

        [TestMethod]
        public void Console_InvalidDelimiter()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
                @"delimiter=5lkj"
            };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(5, container.ReturnCode);
        }

        [TestMethod]
        public void Console_ValidParams_CharCodeDelimiter()
        {
            string[] args =
            {
                @"file=\\My New\File.txt",
                "delimiter=C030",
                "hasHeaders=Y",
            };
            var container = ArgChecker.Check(args);
            Assert.AreEqual("\\\\My New\\File.txt", container.File);
            Assert.AreEqual(char.ConvertFromUtf32(30)[0], container.Delimiter);
            Assert.AreEqual(true, container.HasHeaders);
            Assert.AreEqual("", container.Qualifier);
        }

        [TestMethod]
        public void Console_NoHasHeaders()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
                "delimiter=,",
            };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(6, container.ReturnCode);
        }

        [TestMethod]
        public void Console_InvalidHasHeaders()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
                "delimiter=,",
                "hasHeaders=T",
           };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(7, container.ReturnCode);
        }

        [TestMethod]
        public void Console_NoHeadersAndNoDelimitersPerRow()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
                "delimiter=,",
                "hasHeaders=N",
           };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(8, container.ReturnCode);
        }

        [TestMethod]
        public void Console_ValidParams_NoHeaders()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
                "delimiter=,",
                "hasHeaders=N",
                "delimitersPerRow=14",
           };
            var container = ArgChecker.Check(args);
            Assert.AreEqual("\\\\My\\File.txt", container.File);
            Assert.AreEqual(',', container.Delimiter);
            Assert.AreEqual(false, container.HasHeaders);
            Assert.AreEqual(14, container.DelimitersPerRow);
            Assert.AreEqual("", container.Qualifier);
        }

        [TestMethod]
        public void Console_InvalidDelimitersPerRow()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
                "delimiter=,",
                "hasHeaders=N",
                "delimitersPerRow=S1",
           };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(9, container.ReturnCode);
        }

        [TestMethod]
        public void Console_ValidParams_WithQualifier()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
                "delimiter=,",
                "hasHeaders=Y",
                "qualifier=\"",
            };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(@"\\My\File.txt", container.File);
            Assert.AreEqual(',', container.Delimiter);
            Assert.AreEqual(true, container.HasHeaders);
            Assert.AreEqual("\"", container.Qualifier);
        }

        [TestMethod]
        public void Console_InvalidQualifier()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
                "delimiter=,",
                "hasHeaders=Y",
                "qualifier=Dr",
            };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(10, container.ReturnCode);
        }

        [TestMethod]
        public void Console_ParenthesesQualifier()
        {
            string[] args =
            {
                @"file=\\My\File.txt",
                "delimiter=,",
                "hasHeaders=Y",
                "qualifier=()",
            };
            var container = ArgChecker.Check(args);
            Assert.AreEqual(@"\\My\File.txt", container.File);
            Assert.AreEqual(',', container.Delimiter);
            Assert.AreEqual(true, container.HasHeaders);
            Assert.AreEqual("()", container.Qualifier);
        }
    }
}
