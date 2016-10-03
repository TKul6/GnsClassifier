using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            var resultsDbLocation = ConfigurationManager.AppSettings["resultsFileDbLocation"];
            var contestDbLocation = ConfigurationManager.AppSettings["contestFileDbLocation"];
            _resultsDb = new DictionaryFileDb<string, ClassifierResult>(resultsDbLocation);
            _contestDb = new DictionaryFileDb<string, int>(contestDbLocation);

            _unClassifiedWordCount = _resultsDb.GetEntries().Count(entry => entry.Value == ClassifierResult.Unknown);

            _winnerTracker = new WinnerTracker(_contestDb.GetEntries(), int.Parse(ConfigurationManager.AppSettings["numberOfWinnersToTrack"]));

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
        public int GetPersonalScore()
        {

            var userName = Context.QueryString[Constants.USER_NAME];

            if (_contestDb.GetEntries().ContainsKey(userName))
            {
                return _contestDb.GetEntries()[userName];
            }
            return 0;
        }

        private void UpdateUserContextResults()
        {
            var userName = Context.QueryString[Constants.USER_NAME];
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
                var users = _winnerTracker.TopUsers.ToArray();
                Clients.All.topResultsChanged(users);
            }
        }


        /// <summary>
        /// Gets the <see cref="_unClassifiedWordCount"/>
        /// </summary>
        /// <remarks>
        /// this is for pool only approach, you should subscribe to a notidication in order to get real time update notifications
        /// </remarks>
        /// <returns></returns>
        public long GetUnclassifiedWordsCount()
        {
            return Interlocked.Read(ref _unClassifiedWordCount);
        }

        /// <summary>
        /// Gets the <see cref="_winnerTracker.TopUsers"/>
        /// </summary>
        ///<remarks>
        /// this is for pool only approach, you should subscribe to a notidication in order to get real time update notifications</remarks>
        /// <returns></returns>
        public UserData[] GetTopUsers()
        {
            return _winnerTracker.TopUsers.ToArray();
        }
        
    }
}