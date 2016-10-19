using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace Linq2Objects
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            string text = @"Historically, the world of data and the world of objects" +
              @" have not been well integrated. Programmers work in C# or Visual Basic" +
              @" and also in SQL or XQuery. On the one side are concepts such as classes," +
              @" objects, fields, inheritance, and .NET Framework APIs. On the other side" +
              @" are tables, columns, rows, nodes, and separate languages for dealing with" +
              @" them. Data types often require translation between the two worlds; there are" +
              @" different standard functions. Because the object world has no notion of query, a" +
              @" query can only be represented as a string without compile-time type checking or" +
              @" IntelliSense support in the IDE. Transferring data from SQL tables or XML trees to" +
              @" objects in memory is often tedious and error-prone.";

            CountWords(text);
            FindSentences(text);
            QueryAString("ABCDE99F-J74-12-89A");
            QueryWithRegex();

            // Display execution time
            //Thread.Sleep(1000);
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            Console.WriteLine($"RunTime {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}");

            // Keep console window open in debug mode
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void CountWords(string text)
        {
            string searchTerm = "data";

            //Convert the string into an array of words
            string[] source = text.Split(new [] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Create the query.  Use ToLowerInvariant to match "data" and "Data" 
            var matchQuery = from word in source
                             where word.ToLowerInvariant() == searchTerm.ToLowerInvariant()
                             select word;

            // Count the matches, which executes the query.
            int wordCount = matchQuery.Count();
            Console.WriteLine($"{wordCount} occurrences(s) of the search term \"{searchTerm}\" were found.");
        }

        static void FindSentences(string text)
        {
            string[] sentences = text.Split(new [] {'.', '!', '?'});
            string[] wordToMatch = {"Historically", "data", "integrated"};

            //foreach (var s in sentences)
            //{
            //    foreach (var w in (s.Split(new[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries)).Distinct())
            //        Console.WriteLine(w);
            //}

            var sentenceQuery = from s in sentences
                let w = s.Split(new [] {'.', '?', '!', ' ', ';', ':', ','}, StringSplitOptions.RemoveEmptyEntries)
                where w.Distinct().Intersect(wordToMatch).Count() == wordToMatch.Count()
                select s;

            // Execute the query. Note that you can explicitly type
            // the iteration variable here even though sentenceQuery
            // was implicitly typed. 
            foreach (string str in sentenceQuery)
            {
                Console.WriteLine(str);
            }
        }

        static void QueryAString(string aString)
        {
            // Select only those characters that are numbers
            IEnumerable<char> stringQuery = from ch in aString
                where Char.IsDigit(ch)
                select ch;

            // Execute the query
            foreach (char c in stringQuery)
                Console.Write($"{c} ");
            Console.WriteLine();
            // Call the Count method on the existing query.
            int count = stringQuery.Count();
            Console.WriteLine("Count = {0}", count);

            // Select all characters before the first '-'
            IEnumerable<char> stringQuery2 = aString.TakeWhile(c => c != '-');

            // Execute the second query
            foreach (char c in stringQuery2)
                Console.Write(c);
            Console.WriteLine();

            // Select all characters except '-'
            IEnumerable<char> stringQuery3 = from c in aString
                where c != '-'
                select c;

            // Execute the second query
            foreach (char c in stringQuery3)
                Console.Write(c);
            Console.WriteLine();
        }

        static void QueryWithRegex()
        {
            // Modify this path as necessary so that it accesses your version of Visual Studio.
            string startFolder = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\";

            // Take a snapshot of the file system.
            IEnumerable<System.IO.FileInfo> fileList = Util.GetFiles(startFolder);

            // Create the regular expression to find all things "Visual".
            System.Text.RegularExpressions.Regex searchTerm =
                new System.Text.RegularExpressions.Regex(@"Visual (Basic|C#|C\+\+|J#|SourceSafe|Studio)");

            // Search the contents of each .htm file.
            // Remove the where clause to find even more matchedValues!
            // This query produces a list of files where a match
            // was found, and a list of the matchedValues in that file.
            // Note: Explicit typing of "Match" in select clause.
            // This is required because MatchCollection is not a 
            // generic IEnumerable collection.
            var queryMatchingFiles = from file in fileList
                where file.Extension == ".htm"
                let fileText = System.IO.File.ReadAllText(file.FullName)
                let matches = searchTerm.Matches(fileText)
                where matches.Count > 0
                select new
                {
                    name = file.FullName,
                    matchedValues = from System.Text.RegularExpressions.Match match in matches
                    select match.Value
                };

            // Execute the query.
            Console.WriteLine($"The term \"{searchTerm.ToString()}\" was found in:");

            foreach (var v in queryMatchingFiles)
            {
                // Trim the path a bit, then write 
                // the file name in which a match was found.
                string s = v.name.Substring(startFolder.Length - 1);
                Console.WriteLine(s);

                // For this file, write out all the matching strings
                foreach (var v2 in v.matchedValues)
                {
                    Console.WriteLine("  " + v2);
                }
            }
        }
    }
}
