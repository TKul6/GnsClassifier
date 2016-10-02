using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace GnsClassifier.Common
{
    /// <summary>
    /// Tracks the top users and their scores in the system
    /// </summary>
    public interface IWinnerTracker
    {
        /// <summary>
        /// Updates a new score
        /// </summary>
        /// <param name="user">The user submitted a value</param>
        /// <param name="score">The <see cref="user"/>'s scores</param>
        /// <returns>A boolean indicating rather the top X results has been changed</returns>
        bool Update(string user, int score);

        /// <summary>
        /// Retrieves a list of the top score users
        /// </summary>
        IList<UserData> TopUsers { get; }
    }
}