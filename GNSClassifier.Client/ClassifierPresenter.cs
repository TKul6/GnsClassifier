using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GnsClassifier.Client.Properties;
using GnsClassifier.Common;
using Prism.Commands;

namespace GnsClassifier.Client
{
    public class ClassifierPresenter : INotifyPropertyChanged, IDisposable
    {
        private readonly ClassifierAdapter _adapter;

        private readonly ClassifierHubAdapter _hubAdapter;

        private string _currentWord;
        private bool _isLoading;
        private int _score;
        private int _unclassifiedWordsCount;
        private string _status;

        public string CurrentWord
        {
            get { return _currentWord; }
            set
            {
                if (value == _currentWord) return;
                _currentWord = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (value == _isLoading) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }


        public int Score
        {
            get { return _score; }
            set
            {
                _score = value;
                OnPropertyChanged();
            }
        }

        public int UnclassifiedWordsCount
        {
            get { return _unclassifiedWordsCount; }
            set
            {
                _unclassifiedWordsCount = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserData> TopUsers { get; private set; }

        public ICommand SubmitCommand { get; set; }
        public ICommand GetNewWordCommand { get; set; }

        public ClassifierPresenter()
        {
            var address = ConfigurationManager.AppSettings["serverAddress"];
            _adapter = new ClassifierAdapter(address);
            SubmitCommand = new DelegateCommand<ClassifierResult?>(Submit);
            GetNewWordCommand = new DelegateCommand(GetNewWord);
            _hubAdapter = new ClassifierHubAdapter(ConfigurationManager.AppSettings["signalrHubAddress"]);

            TopUsers = new ObservableCollection<UserData>();

        }

        private void GetNewWord()
        {
            Task.Run(GetNewWordFromServer);
        }

        private async Task GetNewWordFromServer()
        {
            IsLoading = true;

            Thread.Sleep(400);
            CurrentWord = await _hubAdapter.GetNewWordAsync();
            IsLoading = false;


        }

        private async void Submit(ClassifierResult? result)
        {
            if (string.IsNullOrEmpty(CurrentWord))
            {
                return;
            }
            Debug.Assert(result != null, "result != null");
            //_adapter.SubmitResultAsync(CurrentWord, result.Value);
            await _hubAdapter.SubmitResultAsync(CurrentWord, result.Value);

            Score++;

            GetNewWord();
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Getting initialization data from the server
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            IsLoading = true;

            Status = "Connecting to server";

            await _hubAdapter.InitializeConnection().ConfigureAwait(false);


            Status = "Getting personal score";

            Score = await _hubAdapter.GetPersonalScoreAsync().ConfigureAwait(false);

            Status = "Getting Unclassified words count";

            UnclassifiedWordsCount = await _hubAdapter.GetWordCountAsync().ConfigureAwait(false);

            Status = "Getting Top users";

            var users = await _hubAdapter.GetTopUsersAsync().ConfigureAwait(false);

            App.Current.Dispatcher.Invoke(() => { TopUsers.AddRange(users); });

            Status = "Subscribing to Server's events";

            _hubAdapter.TopResultChanged += UpdateTopResults;

            _hubAdapter.UnClassifiedWordCountChanged += UpdateUnClassfiedWordCount;

            Status = "Ready";

            IsLoading = false;
        }

        private void UpdateUnClassfiedWordCount(int newCount)
        {
            UnclassifiedWordsCount = newCount;
        }

        private void UpdateTopResults(UserData[] users)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TopUsers.Clear();

                TopUsers.AddRange(users);
            });
        }

        public void Dispose()
        {
            _hubAdapter.Dispose();
        }
    }
}
