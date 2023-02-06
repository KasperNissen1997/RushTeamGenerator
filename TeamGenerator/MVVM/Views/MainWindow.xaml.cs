using System;
using System.Diagnostics;
using System.Windows;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.MVVM.Views
{
    public partial class MainWindow : Window
    {
        private static MainWindow _instance;
        public static MainWindow Instance
        {
            get
            {
                if (_instance == null)
                    return new MainWindow();

                return _instance;
            }

            private set
            {
                _instance = value;
            }
        }

        public MainMenuView MainMenuView { get; set; }
        public EditPlayersView EditPlayersView { get; set; }
        public GenerateTeamsView GenerateTeamsView { get; set; }

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();

            MainMenuView = new MainMenuView();

            MainFrame.Content = MainMenuView;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Trace.WriteLine("Closing window!");

            if (EditPlayersView != null)
                if (EditPlayersView.DataContext is EditPlayersViewModel editPlayersVM) 
                    editPlayersVM.UpdatePlayerViewModelSources();

            PlayerRepository.Instance.Save();
            TeamRepository.Instance.Save();
        }
    }
}
