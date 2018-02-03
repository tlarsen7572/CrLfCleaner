using System;
using System.IO;
using CrLfCleaner.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrLfCleaner.Tests
{
    [TestClass]
    public class CrLfTests
    {
        [TestMethod]
        public void CleanTestFile01_BasicCase()
        {
            string cleanContent = RunFileWithHeaders("testFile01.txt", '|');
            Assert.AreEqual(
@"Field1|Field2|Field3
Value1|Value2|Value3
Value4|Value5|Value6
", cleanContent
            );
        }

        [TestMethod]
        public void CleanTestFile02_BasicCase()
        {
            string cleanContent = RunFileWithHeaders("testFile02.txt", '|');
            Assert.AreEqual(
@"Field1|Field2|Field3|Field4
Value1|Value2|Value3|Value4
Value5|Value6|Value7|Value8
", cleanContent
            );
        }

        [TestMethod]
        public void CleanTestFile03_DifferentDelimiter()
        {
            Cleaner cleaner = Cleaner.WithHeaders("testFile03.txt", ',');
            cleaner.Clean();

            string cleanContent = File.ReadAllText("testFile03.txtclean");
            Assert.AreEqual(
@"Field1,Field2,Field3
Value1,Value2,Value3
Value4,Value5,Value6
", cleanContent
            );
        }

        [TestMethod]
        public void CleanTestFile04_NoHeader()
        {
            string cleanContent = RunFileNoHeaders("testFile04.txt", ',', 2);
            Assert.AreEqual(
@"Value1,Value2,Value3
Value4,Value5,Value6
", cleanContent
            );
        }

        // In testFile5.txt each record is terminated with \r\n.
        // On the second record, the split between Value and 2 is \n.
        // This test makes sure that mixed line ending schemes are handled properly.
        [TestMethod]
        public void CleanTestFile05_MultipleLineEndings()
        {
            string cleanContent = RunFileWithHeaders("testFile05.txt", ',');
            Assert.AreEqual(
@"Field1,Field2,Field3
Value1,Value2,Value3
Value4,Value5,Value6
", cleanContent
            );
        }

        // In testFile6.txt each field is delimited with a record separator (ASCII int code 30, hex 001E).
        // This test makes sure that control character delimiters are handled properly.
        [TestMethod]
        public void CleanTestFile06_ControlCharacterDelimiters()
        {
            Cleaner cleaner = Cleaner.WithHeaders("testFile06.txt", 30);
            cleaner.Clean();

            string cleanContent = ReadCleanedFile("testFile06.txt");
            Assert.AreEqual(
"Field1\u001EField2\u001EField3\r\n" +
"Value1\u001EValue2\u001EValue3\r\n" +
"Value4\u001EValue5\u001EValue6\r\n", cleanContent
            );
        }

        [TestMethod]
        public void CleanTestFile06_InvalidCharacterCodeProvided()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Cleaner.WithHeaders("testFile06.txt", -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Cleaner.WithHeaders("testFile06.txt", 256));
        }

        [TestMethod]
        public void CleanTestFile07_DelimiterInTextQualifier()
        {
            string cleanContent = RunFileWithQualifiers("testFile07.txt", '|', "\"");
            Assert.AreEqual(
@"""Field1""|""Field2""|""Field3""
""Value1""|""Va|lue2""|""Value3""
""Value4""|""Value5""|""Value6""
", cleanContent
            );
        }

        [TestMethod]
        public void CleanTestFile08_SquareBracketQualifier()
        {
            string cleanContent = RunFileWithQualifiers("testFile08.txt", '|', "[]");
            Assert.AreEqual(
@"[Field1]|[Field2]|[Field3]
[Value1]|[Value|2]|[Value3]
[Value4]|[Value5]|[Value6]
", cleanContent
            );
        }

        [TestMethod]
        public void CleanTestFile09_ParenthesisQualifier()
        {
            string cleanContent = RunFileWithQualifiers("testFile09.txt", '|', "()");
            Assert.AreEqual(
@"(Field1)|(Field2)|(Field3)
(Value1)|(Value|2)|(Value3)
(Value4)|(Value5)|(Value6)
", cleanContent
            );
        }

        [TestMethod]
        public void CleanTestFile10_CurlyBraceQualifier()
        {
            string cleanContent = RunFileWithQualifiers("testFile10.txt", '|', "{}");
            Assert.AreEqual(
@"{Field1}|{Field2}|{Field3}
{Value1}|{Value|2}|{Value3}
{Value4}|{Value5}|{Value6}
", cleanContent
            );
        }

        private string RunFileWithQualifiers(string fileName, char delimiter, string qualifier)
        {
            Cleaner cleaner = Cleaner.WithHeaders(fileName, delimiter, qualifier);
            cleaner.Clean();

            return ReadCleanedFile(fileName);
        }

        private string RunFileWithHeaders(string fileName, char delimiter)
        {
            Cleaner cleaner = Cleaner.WithHeaders(fileName, delimiter);
            cleaner.Clean();

            return ReadCleanedFile(fileName);
        }

        private string RunFileNoHeaders(string fileName, char delimiter, int delimitersPerRow)
        {
            Cleaner cleaner = Cleaner.NoHeaders(fileName, delimiter, delimitersPerRow);
            cleaner.Clean();

            return ReadCleanedFile(fileName);
        }

        private string ReadCleanedFile(string fileName)
        {
            return File.ReadAllText(fileName + "clean");
        }
    }
}
