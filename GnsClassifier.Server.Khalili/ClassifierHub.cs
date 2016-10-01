using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using GnsClassifier.Common;
using Microsoft.AspNet.SignalR;

namespace GnsClassifier.Server.Khalili
{
    public class ClassifierHub : Hub
    {
        private readonly IDictionaryDb<string, ClassifierResult> _resultsDb;
        private readonly IDictionaryDb<string, int> _contestDb;
        private long _unClassifiedWordCount;

        public ClassifierHub()
        {
            var resultsDbLocation = ConfigurationManager.AppSettings["resultsFileDbLocation"];
            var contestDbLocation = ConfigurationManager.AppSettings["contestFileDbLocation"];
            _resultsDb = new DictionaryFileDb<string, ClassifierResult>(resultsDbLocation);
            _contestDb = new DictionaryFileDb<string, int>(contestDbLocation);

            _unClassifiedWordCount = _resultsDb.GetEntries().Count(entry => entry.Value == ClassifierResult.Unknown);
        }

        public void Hello()
        {
            Clients.All.hello();
        }


        public string GetWordToClassify()
        {
            var words = GetUnknownWords();
            if (!words.Any())
            {
                return "";
            }
            var randomizer = new Random();
            var index = randomizer.Next(words.Count);
            var word = words[index];
            return word;
        }

        private IList<string> GetUnknownWords()
        {
            var wordsResults = _resultsDb.GetEntries();
            var unknownWords = wordsResults
                .Where(kvp => kvp.Value == ClassifierResult.Unknown)
                .Select(kvp => kvp.Key).ToList();
            return unknownWords;
        }

        public void UpdateUnclassifiedCount()
        {
            Clients.All.updateUnclassifiedCount(Interlocked.Read(ref _unClassifiedWordCount));
        }

        public void SubmitClassifierResult(string word, ClassifierResult result)
        {
            if (string.IsNullOrEmpty(word))
            {
                return;
            }
            _resultsDb.UpdateEntry(word, result);

            Interlocked.Decrement(ref _unClassifiedWordCount);

            UpdateUnclassifiedCount();

            UpdateUserContextResults();
        }



        private void UpdateUserContextResults()
        {
          var userName = Context.User.Identity.Name;
            var contestResults = _contestDb.GetEntries();
            var userScore = 1;
            if (contestResults.ContainsKey(userName))
            {
                userScore = contestResults[userName] + 1;
            }
            _contestDb.UpdateEntry(userName, userScore);
        }
    }
}