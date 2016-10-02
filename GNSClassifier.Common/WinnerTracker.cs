using System;
using System.Collections.Generic;
using System.Linq;

namespace GnsClassifier.Common
{
    /// <summary>
    /// A basic implementation of <see cref="IWinnerTracker"/>
    /// </summary>
    public class WinnerTracker : IWinnerTracker
    {
        #region Data Members
        /// <summary>
        /// a list contating all <see cref="UserData"/> in a descending order by the user's score
        /// </summary>
        private IList<UserData> _topUsers;

        /// <summary>
        /// Indicating how many scores counts a s a top score 
        /// </summary>
        //TODO: maybe should be const 6
        private int _numOfUsersToTrack;

        private object _syncObject;
        #endregion

        #region Constructor
        public WinnerTracker(IDictionary<string, int> usersData, int numOfUsersToTrack)
        {
            ValidateArguments(usersData, numOfUsersToTrack);

            _numOfUsersToTrack = numOfUsersToTrack;

            _topUsers = usersData.OrderByDescending(score => score.Value).Take(_numOfUsersToTrack).Select(score => new UserData(score.Key, score.Value)).ToList();

            _syncObject = new object();
        }

        private static void ValidateArguments(IDictionary<string, int> usersData, int numOfUsersToTrack)
        {
            if (usersData == null)
            {
                throw new ArgumentException("Parameter can not be null", nameof(usersData));
            }

            if (numOfUsersToTrack <= 0)
            {
                throw new ArgumentException("Parameter must be greater than zero", nameof(numOfUsersToTrack));
            }
        }
        #endregion

        /// <summary>
        /// Updates a new score
        /// </summary>
        /// <param name="user">The user submitted a value</param>
        /// <param name="score">The <see cref="user"/>'s scores</param>
        /// <returns>A boolean indicating rather the top X results has been changed</returns>
        public bool Update(string user, int score)
        {
            if (_topUsers.Count < _numOfUsersToTrack)
            {
                InnerUpdateScore(user, score);

                return true;
            }

            if (score > _topUsers.ElementAt(_numOfUsersToTrack - 1).Score)
            {
                InnerUpdateScore(user, score);

                return true;
            }

            return false;
        }


        /// <summary>
        /// Update the score in the right place in the  <see cref="_topUsers"/>
        /// </summary>
        /// <param name="user">The user submitted a value</param>
        /// <param name="score">The <see cref="user"/>'s scores</param>
        private void InnerUpdateScore(string user, int score)
        {
            int index = 0;

            bool userExist = false;

            lock (_syncObject)
            {
                while (_topUsers[index].Score > score && index < _topUsers.Count && !userExist)
                {
                    if (_topUsers[index].Username.Equals(user))
                    {
                        _topUsers[index].IncrementScore();

                        userExist = true;
                    }

                    index++;
                }
                
                if (!userExist)
                {
                    _topUsers.Insert(index, new UserData(user, score));
                } 
            }
            
        }

        /// <summary>
        /// Retrieves a list of the top score users
        /// </summary>
        public IList<UserData> TopUsers
        {
            get { return _topUsers.Take(_numOfUsersToTrack).ToList(); }
        }
    }
}