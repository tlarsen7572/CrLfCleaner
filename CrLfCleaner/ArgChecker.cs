using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CrLfCleaner
{
    public struct ArgContainer
    {
        public ArgContainer(string file, bool hasHeaders, int delimitersPerRow, char delimiter, string qualifier) : 
            this(0, file, hasHeaders, delimitersPerRow, delimiter, qualifier) { }
        public static ArgContainer Error(int returnCode)
        {
            if (returnCode == 0) throw new NotSupportedException("An error ArgContainer cannot contain a return code of 0");
            return new ArgContainer(
                returnCode,
                file: "",
                hasHeaders: false,
                delimitersPerRow: 0,
                delimiter: Char.MinValue,
                qualifier: ""
            );
        }
        private ArgContainer(int returnCode,string file,bool hasHeaders,int delimitersPerRow,char delimiter,string qualifier)
        {
            ReturnCode = returnCode;
            File = file;
            HasHeaders = hasHeaders;
            DelimitersPerRow = delimitersPerRow;
            Delimiter = delimiter;
            Qualifier = qualifier;
        }
        public readonly int ReturnCode;
        public readonly string File;
        public readonly bool HasHeaders;
        public readonly int DelimitersPerRow;
        public readonly char Delimiter;
        public readonly string Qualifier;
    }

    public class ArgChecker
    {
        static public ArgContainer Check(string[] args)
        {
            // Make sure args were provided
            if (args.Length == 0)
            {
                Console.WriteLine(noArgs);
                return ArgContainer.Error(1);
            }

            var file = GetArgValue("file=", args);
            if (ArgDoesNotExist(file, noFile)) return ArgContainer.Error(2);

            var delimiter = GetArgValue("delimiter=", args);
            if (ArgDoesNotExist(delimiter, noDelimiter)) return ArgContainer.Error(4);
            if (ArgFormatIsBad(delimiter, "^(.|C[0-9]{3})$", invalidDelimiter)) return ArgContainer.Error(5);
            if (delimiter.Length > 1) // Convert the specified character code into the specified character
            {
                int code = int.Parse(delimiter.Substring(1));
                delimiter = char.ConvertFromUtf32(code);
            }

            var hasHeaders = GetArgValue("hasHeaders=", args).ToUpper();
            if (ArgDoesNotExist(hasHeaders, noHasHeaders)) return ArgContainer.Error(6);
            if (ArgFormatIsBad(hasHeaders, "^(Y|N)$", invalidHasHeaders)) return ArgContainer.Error(7);

            var delimitersPerRow = GetArgValue("delimitersPerRow=", args);
            if (hasHeaders == "N")
            {
                if (ArgDoesNotExist(delimitersPerRow, noDelimitersPerRow)) return ArgContainer.Error(8);
                if (ArgFormatIsBad(delimitersPerRow, "^[0-9]+$", invalidDelimitersPerRow)) return ArgContainer.Error(9);
            }

            var qualifier = GetArgValue("qualifier=", args);
            if (qualifier != string.Empty)
            {
                if (ArgFormatIsBad(qualifier, @"^(.|\[\]|\(\)|\{\})$", invalidQualifier)) return ArgContainer.Error(10);
            }
                
            return new ArgContainer(
                file: file,
                hasHeaders: hasHeaders == "Y" ? true : false,
                delimitersPerRow: hasHeaders == "N" ? int.Parse(delimitersPerRow) : 0,
                delimiter: delimiter[0],
                qualifier: qualifier
            );
        }

        static private string GetArgValue(string param, string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.Length >= param.Length && arg.Substring(0, param.Length).Equals(param))
                {
                    return arg.Substring(param.Length);
                }
            }
            return String.Empty;
        }

        static private bool ArgDoesNotExist(string value, string errorMsg)
        {
            if (value.Equals(String.Empty))
            {
                Console.WriteLine(errorMsg);
                return true;
            }
            return false;
        }

        static private bool ArgFormatIsBad(string value, string pattern, string errorMsg)
        {
            if (!Regex.Match(value, pattern).Success)
            {
                Console.WriteLine(errorMsg);
                return true;
            }
            return false;
        }

        const string noFile = "The file parameter was not provided and is a required parameter.  Execution halted.";
        const string invalidFile = "The file parameter is invalid.  Execution halted.";
        const string noDelimiter = "The delimiter parameter was not provided and is a required parameter.  Execution halted.";
        const string invalidDelimiter = "The delimiter parameter is invalid.  Provide either a single character or an ASCII character code formatted as a capital C followed by three digits (i.e. C032).";
        const string noHasHeaders = "The hasHeaders parameter was not provided and is a required parameter.  Execution halted";
        const string invalidHasHeaders = "The hasHeaders parameter is invalid.  Provide either Y for yes or N for no.  Execution halted.";
        const string noDelimitersPerRow = "The delimitersPerRow parameter was not provided but hasHeaders is N (no).  The delimitersPerRow parameter must be provided when hasHeaders is N.  Execution halted.";
        const string invalidDelimitersPerRow = "The delimitersPerRow parameter must be a numeric value.  Execution halted.";
        const string invalidQualifier = "The qualifier parameter must be a single character, parentheses, square brackets, or curly braces.  Execution halted.";

        const string noArgs = @"No parameters were provided so the cleaner cannot run.

The following parameters are required:
file=<fileName>    : Specifies the relative or absolute path to the file.
                     Quoting paths is allowed.
delimiter=<char>   : The delimiter.  Must be a single character
  OR
delimiter=C<###>   : Specifies an ASCII character code for the delimeter.
                     Must be a three-digit number (i.e. 001, not 1)
                     between 0 and 255.
hasHeaders=<Y|N>   : Identifies whether the data file has a header row.
                     Yes (Y) or No (N)
delimitersPerRow=# : If hasHeaders is No (N), this parameter is required
                     to identify how many delimiters should be in every
                     row.

The following parameter is optional:
qualifier=<string> : Identifies a text qualifier.  Delimiters inside
                     text qualifiers are not counted as delimiters.
                     Must be a single character or one of the following
                     sets of characters:
                       ()
                       []
                       {}

Wrap the entire parameter in double quotes, including the name and equal sign.
If a parameter uses a double quote (e.g. the qualifier parameter), specify two
double quotes when defining the parameter.

Example:
A file called 'MyData.csv' uses comma delimiters and double-quote text
qualifiers.  It also contains a header row.  To clean this file, use
the following call:

CrLfCleaner ""file=MyData.csv"" ""delimiter=,"" ""hasHeaders=Y"" ""qualifier=""""""

Example:
""C:\Users\Me\Documents\My Data\Data.txt"" uses field separator
delimiters (ASCII character code 28) and no text qualifiers.  It
does not contain a header row but should have 15 delimiters in each
row of data.  To clean this file, use the following call:

CrLfCleaner ""file=C:\Users\Me\Documents\My Data\Data.txt"" ""delimiter=C028"" ""hasHeaders=N"" ""delimitersPerRow=15""
";
    }
}
