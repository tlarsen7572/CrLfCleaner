using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CrLfCleaner.Core;

namespace CrLfCleaner
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine();
            ArgContainer a = ArgChecker.Check(args);
            if (a.ReturnCode != 0) return a.ReturnCode;

            Cleaner cleaner;
            if (a.HasHeaders)
            {
                if (a.Qualifier == String.Empty)
                {
                    cleaner = Cleaner.WithHeaders(a.File, a.Delimiter);
                }
                else
                {
                    cleaner = Cleaner.WithHeaders(a.File, a.Delimiter, a.Qualifier);
                }
            } else
            {
                if (a.Qualifier == String.Empty)
                {
                    cleaner = Cleaner.NoHeaders(a.File, a.Delimiter, a.DelimitersPerRow);
                }
                else
                {
                    cleaner = Cleaner.NoHeaders(a.File, a.Delimiter, a.DelimitersPerRow, a.Qualifier);
                }
            }

            Console.WriteLine("CrLfCleaner is running with the following parameters:");
            Console.WriteLine($"File: {cleaner.file}");
            Console.WriteLine($"Delimiter: {cleaner.delimiter}");
            Console.WriteLine($"Qualifier was provided: {cleaner.qualifierProvided}");
            Console.WriteLine($"Starting qualifier: {cleaner.startQualifier}");
            Console.WriteLine($"Ending qualifier: {cleaner.endQualifier}");
            Console.WriteLine($"Has headers: {cleaner.hasHeaders}");
            Console.WriteLine("");

            var start = DateTime.UtcNow;
            try
            {
                cleaner.Clean();
            }
            catch (Exception ex)
            {
                Console.WriteLine("The following error occurred while parsing:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                WriteDuration(start, DateTime.UtcNow);
                return 11;
            }

            Console.WriteLine($"{a.File} has been cleaned.  You can find the cleaned data in {a.File}cleaned.");
            Console.WriteLine();
            WriteDuration(start, DateTime.UtcNow);
            return 0;
        }

        static private void WriteDuration(DateTime start,DateTime end)
        {
            var delta = end.Subtract(start);
            int minutes = Convert.ToInt32(delta.TotalMinutes);
            Console.WriteLine($"Execution took {minutes} minutes and {delta.Seconds} seconds.");
        }
    }
}
