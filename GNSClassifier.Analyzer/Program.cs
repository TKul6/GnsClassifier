using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using GnsClassifier.Common;
using Newtonsoft.Json;

namespace GnsClassifier.Analyzer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        static void Main(string[] args)
        {
            var resultsLocation = ConfigurationManager.AppSettings["resultsLocation"];
            var results = GetAllResults(resultsLocation);
            var wordsCount = results.Count;
            var unknownCount = results.Count(result => result.Value == ClassifierResult.Unknown);
            var unsafeCount = results.Count(result => result.Value == ClassifierResult.Unsafe);
            var safeCount = results.Count(result => result.Value == ClassifierResult.Safe);

            var contestLocation = ConfigurationManager.AppSettings["contestLocation"];
            var contest = GetContest(contestLocation);
            var top5 = contest.OrderBy(kvp => kvp.Value).Take(5);


            Console.WriteLine($"Total words: {wordsCount}");
            Console.WriteLine($"Total unknown words: {unknownCount}");
            Console.WriteLine($"Total unsafe words: {unsafeCount}");
            Console.WriteLine($"Total safe words: {safeCount}");
            Console.WriteLine("Top 5 Users:");
            foreach (var kvp in top5)
            {
                Console.WriteLine($"\tUser: {kvp.Key}, Words Done: {kvp.Value}");
            }
            Console.Read();
        }

        private static IDictionary<string, ClassifierResult> GetAllResults(string location)
        {
            var resultsString = File.ReadAllText(location);
            var results = JsonConvert.DeserializeObject<Dictionary<string, ClassifierResult>>(resultsString);
            return results;
        }

        private static IDictionary<string, int> GetContest(string location)
        {
            var contestString = File.ReadAllText(location);
            var contest = JsonConvert.DeserializeObject<Dictionary<string, int>>(contestString);
            return contest;
        }
    }
}
