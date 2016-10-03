using System;
using MahApps.Metro.Controls;

namespace GnsClassifier.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void InitializePresenter(object sender, EventArgs e)
        {
            await ClassifierPresenter.Initialize();
        }

        private void DispodePresenter(object sender, EventArgs e)
        {
             ClassifierPresenter.Dispose();
        }
    }
}
