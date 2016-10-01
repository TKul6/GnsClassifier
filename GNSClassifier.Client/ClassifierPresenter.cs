using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GnsClassifier.Client.Properties;
using GnsClassifier.Common;
using Prism.Commands;

namespace GnsClassifier.Client
{
    public class ClassifierPresenter : INotifyPropertyChanged
    {
        private readonly ClassifierAdapter _adapter;
        private string _currentWord;
        private bool _isLoading;

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

        public ICommand SubmitCommand { get; set; }
        public ICommand GetNewWordCommand { get; set; }

        public ClassifierPresenter()
        {
            var address = ConfigurationManager.AppSettings["serverAddress"];
            _adapter = new ClassifierAdapter(address);
            SubmitCommand = new DelegateCommand<ClassifierResult?>(Submit);
            GetNewWordCommand = new DelegateCommand(GetNewWord);
        }

        private void GetNewWord()
        {
            Task.Run((Action)GetNewWordFromServer);
        }

        private void GetNewWordFromServer()
        {
            IsLoading = true;
            Thread.Sleep(400);
            CurrentWord = _adapter.GetWordToClassify();
            IsLoading = false;
        }

        private void Submit(ClassifierResult? result)
        {
            if (string.IsNullOrEmpty(CurrentWord))
            {
                return;
            }
            Debug.Assert(result != null, "result != null");
            _adapter.SubmitResult(CurrentWord, result.Value);
            GetNewWord();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
