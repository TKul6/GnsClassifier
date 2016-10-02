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
        private IWinnerTracker _winnerTracker;
        private Random _randomizer;

        /// <summary>
        /// The number of unclassified words
        /// </summary>
        private long _unClassifiedWordCount;
        
        public ClassifierHub()
        {
            //Todo: add this settings to the config file.
            var resultsDbLocation = ConfigurationManager.AppSettings["resultsFileDbLocation"];
            var contestDbLocation = ConfigurationManager.AppSettings["contestFileDbLocation"];
            _resultsDb = new DictionaryFileDb<string, ClassifierResult>(resultsDbLocation);
            _contestDb = new DictionaryFileDb<string, int>(contestDbLocation);

            _unClassifiedWordCount = _resultsDb.GetEntries().Count(entry => entry.Value == ClassifierResult.Unknown);

            _winnerTracker = new WinnerTracker(_contestDb.GetEntries(),int.Parse(ConfigurationManager.AppSettings["numberOfWinnersToTrack"]));

            _randomizer = new Random();
        }


        public string GetWordToClassify()
        {
            var words = GetUnknownWords();
            if (!words.Any())
            {
                return "";
            }
           
            var index = _randomizer.Next(words.Count);
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

        /// <summary>
        /// Calls a callback in the client that updates the number of unclassified words
        /// </summary>
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

        /// <summary>
        /// Calls a callback in the client that updates his personal score
        /// </summary>
        public void GetPersonalScore()
        {
            Clients.Caller.getPersonalResult(_contestDb.GetEntries()[Context.User.Identity.Name]);
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

            UpdateTopScores(userName, userScore);
            
        }

        /// <summary>
        /// Updates the <see cref="_winnerTracker"/> and the users if necessary
        /// </summary>
        /// <param name="userName">The current user</param>
        /// <param name="userScore">The <see cref="userName"/>'s score</param>
        private void UpdateTopScores(string userName, int userScore)
        {
            if (_winnerTracker.Update(userName, userScore))
            {

                Clients.All.topResultsChanged(_winnerTracker.TopUsers);
            }
        }
    }
}