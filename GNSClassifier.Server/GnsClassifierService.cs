using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using GnsClassifier.Common;

namespace GnsClassifier.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GnsClassifierService : IClassifierService
    {
        private readonly IDictionaryDb<string, ClassifierResult> _resultsDb;
        private readonly IDictionaryDb<string, int> _contestDb;

        public GnsClassifierService()
        {
            var resultsDbLocation = ConfigurationManager.AppSettings["resultsFileDbLocation"];
            var contestDbLocation = ConfigurationManager.AppSettings["contestFileDbLocation"];
            _resultsDb = new DictionaryFileDb<string, ClassifierResult>(resultsDbLocation);
            _contestDb = new DictionaryFileDb<string, int>(contestDbLocation);
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

        public void SubmitClassifierResult(string word, ClassifierResult result)
        {
            if (string.IsNullOrEmpty(word))
            {
                return;
            }
            _resultsDb.UpdateEntry(word, result);
            UpdateUserContextResults();
        }

        private void UpdateUserContextResults()
        {
            var securityContext = OperationContext.Current.ServiceSecurityContext;
            var userName = securityContext.PrimaryIdentity.Name;
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
