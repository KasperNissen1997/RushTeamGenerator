using System;
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
            EditPlayersView = new EditPlayersView();
            GenerateTeamsView = new GenerateTeamsView();

            MainFrame.Content = MainMenuView;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (EditPlayersView.DataContext is EditPlayersViewModel editPlayersVM) 
                editPlayersVM.UpdatePlayerViewModelSources();

            if (GenerateTeamsView.DataContext is GenerateTeamsViewModel generateTeamsVM)
                generateTeamsVM.UpdateTeamViewModelSources();

            PlayerRepository.Instance.Save();
            TeamRepository.Instance.Save();
        }
    }
}
