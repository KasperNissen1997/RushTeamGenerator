using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGenerator.Commands;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.Models;

namespace TeamGenerator.MVVM.ViewModels
{
    public class GenerateTeamsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TeamViewModel> _generatedTeams;
        public ObservableCollection<TeamViewModel> GeneratedTeams
        {
            get 
            { 
                return _generatedTeams; 
            }

            set 
            { 
                _generatedTeams = value;
                OnPropertyChanged(nameof(GeneratedTeams));
            }
        }

        private int _teamCapacity = 5;
        public int TeamCapacity
        {
            get
            {
                return _teamCapacity;
            }

            set
            {
                _teamCapacity = value;
                OnPropertyChanged(nameof(TeamCapacity));
            }
        }
        private int _allowedRatingDeviance = 5;
        public int AllowedRatingDeviance
        {
            get
            {
                return _allowedRatingDeviance;
            }

            set
            {
                _allowedRatingDeviance = value;
                OnPropertyChanged(nameof(AllowedRatingDeviance));
            }
        }

        private ObservableCollection<PlayerViewModel> _registeredPlayers;
        public ObservableCollection<PlayerViewModel> RegisteredPlayers
        {
            get
            {
                return _registeredPlayers;
            }

            set
            {
                _registeredPlayers = value;
                OnPropertyChanged(nameof(RegisteredPlayers));
            }
        }
        private PlayerViewModel _selectedPlayer;
        public PlayerViewModel SelectedPlayer
        {
            get
            {
                return _selectedPlayer;
            }

            set
            {
                _selectedPlayer = value;
                OnPropertyChanged(nameof(SelectedPlayer));
            }
        }

        #region Commands
        public GoToMainMenuCommand GoToMainMenuCommand { get; set; } = new();

        public SelectAllPlayersInGeneratorViewCommand SelectAllPlayersInGeneratorViewCommand { get; set; } = new();
        public GenerateTeamsCommand GenerateTeamsCommand { get; set; } = new();
        #endregion

        #region Interface
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged is not null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
        #endregion

        public GenerateTeamsViewModel()
        {
            GeneratedTeams = new();

            foreach (Team team in TeamRepository.Instance.RetrieveAll())
            {
                TeamViewModel teamVM = new TeamViewModel(team);
                GeneratedTeams.Add(teamVM);
            }

            RegisteredPlayers = new();

            foreach (Player player in PlayerRepository.Instance.RetrieveAll())
            {
                PlayerViewModel playerVM = new PlayerViewModel(player);
                RegisteredPlayers.Add(playerVM);
            }
        }

        public void UpdateTeamViewModelSources()
        {
            //throw new NotImplementedException();
        }
    }
}
