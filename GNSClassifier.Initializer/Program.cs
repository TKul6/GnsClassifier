using System;
using System.Configuration;

// ReSharper disable ClassNeverInstantiated.Global

namespace GnsClassifier.Initializer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var wordsLocation = ConfigurationManager.AppSettings["wordsFileLocation"];
            var resultsLocation = ConfigurationManager.AppSettings["resultsFileLocation"];
            var initializer = new Initializer();
            Console.WriteLine($"Reading from file {wordsLocation}");
            var dictionary = initializer.CreateClassifierResultsDictionaryFromFile(wordsLocation);
            Console.WriteLine($"Read {dictionary.Count} words from the file");
            Console.WriteLine($"Writing {dictionary.Count} default results to the file {resultsLocation}");
            initializer.WriteDictionaryToFile(dictionary, resultsLocation);
            Console.WriteLine("Finished writing all results");
            Console.Read();
        }

    }
}
