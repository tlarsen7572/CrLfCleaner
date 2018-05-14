using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CrLfCleaner.Core
{
    public class ParseException : Exception
    {
        public ParseException() : base() { }
        public ParseException(string message) : base(message) { }
        public ParseException(string message, Exception inner) : base(message, inner) { }
    }

    public class Cleaner
    {
        private Cleaner(string file, char delimiter, bool hasHeaders, int delimitersPerRow, string qualifier = "")
        {
            this.file = file;
            this.delimiter = delimiter;
            this.hasHeaders = hasHeaders;
            _delimitersPerRow = delimitersPerRow;

            if (qualifier.Equals(string.Empty))
            {
                qualifierProvided = false;
            }
            else
            {
                if (qualifier.Length > 2 || !Regex.Match(qualifier, @"^(.|\(\)|\[\]|\{\})?$").Success)
                    throw new ArgumentException("qualifier must be a single character, parenthesis, square brackets, or curly braces");

                qualifierProvided = true;
                startQualifier = qualifier[0];
                if (qualifier.Length == 2)
                {
                    endQualifier = qualifier[1];
                }
                else
                {
                    endQualifier = qualifier[0];
                }                    
            }
        }

        // Lots of factory constructors here.  Not sure if I like this...
        public static Cleaner NoHeaders(string file, char delimiter, int delimitersPerRow, string qualifier = "")
        {
            return new Cleaner(file, delimiter, false, delimitersPerRow, qualifier);
        }
        public static Cleaner NoHeaders(string file, char delimiter, int delimitersPerRow)
        {
            return new Cleaner(file, delimiter, false, delimitersPerRow);
        }
        public static Cleaner NoHeaders(string file, int charCode, int delimitersPerRow, string qualifier = "")
        {
            char delimiter = CharCodeToChar(charCode);
            return new Cleaner(file, delimiter, false, delimitersPerRow, qualifier);
        }
        public static Cleaner NoHeaders(string file, int charCode, int delimitersPerRow)
        {
            char delimiter = CharCodeToChar(charCode);
            return new Cleaner(file, delimiter, false, delimitersPerRow);
        }
        public static Cleaner WithHeaders(string file, char delimiter, string qualifier = "")
        {
            return new Cleaner(file, delimiter, true, 0, qualifier);
        }
        public static Cleaner WithHeaders(string file, char delimiter)
        {
            return new Cleaner(file, delimiter, true, 0);
        }
        public static Cleaner WithHeaders(string file, int charCode, string qualifier = "")
        {
            char delimiter = CharCodeToChar(charCode);
            return new Cleaner(file, delimiter, true, 0, qualifier);
        }
        public static Cleaner WithHeaders(string file, int charCode)
        {
            char delimiter = CharCodeToChar(charCode);
            return new Cleaner(file, delimiter, true, 0);
        }

        readonly public string file;
        readonly public bool hasHeaders;
        readonly public char delimiter;
        readonly public char startQualifier;
        readonly public char endQualifier;
        readonly public bool qualifierProvided;
        private bool _isQualified;
        private int _delimitersPerRow;

        public void Clean()
        {
            _isQualified = false;
            StreamReader reader = new StreamReader(file);
            StreamWriter writer = new StreamWriter(file + "clean");
            int currentRow = 0;

            if (hasHeaders)
            {
                currentRow++;
                ProcessHeader(reader, writer);
            }

            string line;
            int delimiters = 0;
            while ((line = reader.ReadLine()) != null)
            {
                currentRow++;
                delimiters = delimiters + CountDelimiters(line);

                if (delimiters > _delimitersPerRow)
                {
                    CleanUpState(reader, writer);
                    throw new ParseException($"Cleaner found more delimiters than allowed per row while processing row {currentRow}.  Parsing failed.");
                }

                writer.Write(line);
                if (delimiters == _delimitersPerRow)
                {
                    writer.Write(Environment.NewLine);
                    delimiters = 0;
                }
            }

            CleanUpState(reader, writer);
        }

        private void ProcessHeader(StreamReader reader, StreamWriter writer)
        {
            string line = reader.ReadLine();
            if (line != null)
            {
                _delimitersPerRow = CountDelimiters(line);
                writer.WriteLine(line);
            }
        }

        private int CountDelimiters(String line)
        {
            int delimiters = 0;
            foreach (char c in line)
            {
                if (qualifierProvided)
                {
                    if (_isQualified)
                    {
                        if (c.Equals(endQualifier)) _isQualified = false;
                    }
                    else
                    {
                        if (c.Equals(startQualifier)) _isQualified = true;
                    }                        
                }
                if (c.Equals(delimiter) && !_isQualified) delimiters++;
            }
            return delimiters;
        }

        private void CleanUpState(StreamReader reader, StreamWriter writer)
        {
            writer.Flush();
            writer.Dispose();
            reader.Dispose();
            _isQualified = false;
        }

        private static char CharCodeToChar(int charCode)
        {
            if (charCode < 0 || charCode > 255) throw new ArgumentOutOfRangeException("charCode must be between 0 and 255");
            return char.ConvertFromUtf32(charCode)[0];
        }
    }
}