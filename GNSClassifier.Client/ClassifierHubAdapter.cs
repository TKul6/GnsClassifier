using System;
using System.Threading.Tasks;
using GnsClassifier.Common;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System.Collections.Generic;

namespace GnsClassifier.Client
{

    /// <summary>
    /// Forwards http calls to the SignalR hub
    /// </summary>
    public class ClassifierHubAdapter : IDisposable
    {
        #region Data Members
        /// <summary>
        /// A proxy to the SignalR server
        /// </summary>
        private IHubProxy _proxy;

        /// <summary>
        /// Connection to connect to the server and create the <see cref="_proxy"/>
        /// </summary>
        private HubConnection _hubConnection;

        /// <summary>
        /// Occures when the top users chart in the serever updated
        /// </summary>
        public event Action<UserData[]> TopResultChanged;

        /// <summary>
        /// Occurs when the number of unclassified words in the server updated;
        /// </summary>
        public event Action<int> UnClassifiedWordCountChanged;

        /// <summary>
        /// Contains all the disposable subscription to the server
        /// </summary>
        private List<IDisposable> _disposables;
        #endregion

        #region Constructor
        public ClassifierHubAdapter(string hubUrl)
        {
            _hubConnection = new HubConnection(hubUrl, new Dictionary<string, string>() { { Constants.USER_NAME, Environment.MachineName } });

            _proxy = _hubConnection.CreateHubProxy("classifierHub");

            _disposables = new List<IDisposable>();

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize the connection with the signalR server and register to events
        /// </summary>
        /// <returns></returns>
        public async Task InitializeConnection()
        {
            _disposables.Add(_proxy.On<UserData[]>(Constants.TOP_RESULTS_CHANGED, OnTopResultChanged));
            _disposables.Add(_proxy.On<int>(Constants.UPDATE_UNCLASSIFIED_COUNT, OnUnClassifiedWordCountChanged));
            await _hubConnection.Start().ConfigureAwait(false);

        }

        /// <summary>
        /// Gets new unclassified word from the server
        /// </summary>
        /// <returns></returns>
        public Task<string> GetNewWordAsync()
        {
            return _proxy.Invoke<string>("GetWordToClassify");
        }

        /// <summary>
        /// Gets the user's score from the server
        /// </summary>
        /// <returns></returns>
        public Task<int> GetPersonalScoreAsync()
        {
            return _proxy.Invoke<int>("GetPersonalScore");
        }


        /// <summary>
        /// Submit new word
        /// </summary>
        /// <param name="currentWord">The word to submit</param>
        /// <param name="value">The word's state (either <see cref="ClassifierResult.Safe"/> or <see cref="ClassifierResult.Unsafe"/></param>
        /// <returns></returns>
        public Task SubmitResultAsync(string currentWord, ClassifierResult value)
        {
            return _proxy.Invoke("SubmitClassifierResult", currentWord, value);
        }

        /// <summary>
        /// Gets the number of unclassified words from the sever
        /// </summary>
        /// <returns></returns>
        public Task<int> GetWordCountAsync()
        {
            return _proxy.Invoke<int>("GetUnclassifiedWordsCount");
        }

        /// <summary>
        /// Gets the top user's and their scores from the server
        /// </summary>
        /// <returns></returns>
        public Task<UserData[]> GetTopUsersAsync()
        {
            return _proxy.Invoke<UserData[]>("GetTopUsers");
        } 
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Disposes all subscription to the server and the connection
        /// </summary>
        public void Dispose()
        {
            TopResultChanged = null;

            UnClassifiedWordCountChanged = null;

            _hubConnection.Stop();

            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            _hubConnection.Dispose();
            
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Invokes the <see cref="TopResultChanged"/>
        /// </summary>
        /// <param name="newUsers"></param>
        private void OnTopResultChanged(UserData[] newUsers)
        {
            TopResultChanged?.Invoke(newUsers);
        }

        /// <summary>
        /// Invokes the <see cref="UnClassifiedWordCountChanged"/>
        /// </summary>
        /// <param name="newCount"></param>
        private void OnUnClassifiedWordCountChanged(int newCount)
        {
            UnClassifiedWordCountChanged?.Invoke(newCount);
        } 
        #endregion

    }
}