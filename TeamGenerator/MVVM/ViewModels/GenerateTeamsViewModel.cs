using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TeamGenerator.Commands;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.Models;
using TeamGenerator.Commands.GenerateTeamsViewCommands;
using System.Collections.Specialized;

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

        private ObservableCollection<PlayerViewModel> _leftOverPlayers;
        public ObservableCollection<PlayerViewModel> LeftOverPlayers
        {
            get
            {
                return _leftOverPlayers;
            }

            set
            {
                _leftOverPlayers = value;
                OnPropertyChanged(nameof(LeftOverPlayers));
            }
        }

        private int _pageNumber = 0;
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }

            set
            {
                _pageNumber = value;

                OnPropertyChanged(nameof(UpperLeftTeamName));
                OnPropertyChanged(nameof(UpperLeftTeam));

                OnPropertyChanged(nameof(UpperRightTeamName));
                OnPropertyChanged(nameof(UpperRightTeam));

                OnPropertyChanged(nameof(LowerLeftTeamName));
                OnPropertyChanged(nameof(LowerLeftTeam));

                OnPropertyChanged(nameof(LowerRightTeamName));
                OnPropertyChanged(nameof(LowerRightTeam));
            }
        }

        #region Team Properties
        public string UpperLeftTeamName
        {
            get
            {
                int index = 0 + 4 * PageNumber;

                try {
                    if (GeneratedTeams[index] != null)
                        return $"Team {index + 1}";

                    return "Team ?";
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return "Team ?";
                }
            }
        }
        public TeamViewModel UpperLeftTeam
        {
            get
            {
                int index = 0 + 4 * PageNumber;

                try
                {
                    if (GeneratedTeams[index] != null)
                        return GeneratedTeams[index];

                    return null;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return null;
                }
            }
        }

        public string UpperRightTeamName
        {
            get
            {
                int index = 1 + 4 * PageNumber;

                try
                {
                    if (GeneratedTeams[index] != null)
                        return $"Team {index + 1}";

                    return "Team ?";
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return "Team ?";
                }
            }

        }
        public TeamViewModel UpperRightTeam
        {
            get
            {
                int index = 1 + 4 * PageNumber;

                try
                {
                    if (GeneratedTeams[index] != null)
                        return GeneratedTeams[index];

                    return null;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return null;
                }
            }
        }

        public string LowerLeftTeamName
        {
            get
            {
                int index = 2 + 4 * PageNumber;

                try
                {
                    if (GeneratedTeams[index] != null)
                        return $"Team {index + 1}";

                    return "Team ?";
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return "Team ?";
                }
            }
        }
        public TeamViewModel LowerLeftTeam
        {
            get
            {
                int index = 2 + 4 * PageNumber;

                try
                {
                    if (GeneratedTeams[index] != null)
                        return GeneratedTeams[index];

                    return null;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return null;
                }
            }
        }

        public string LowerRightTeamName
        {
            get
            {
                int index = 3 + 4 * PageNumber;

                try
                {
                    if (GeneratedTeams[index] != null)
                        return $"Team {index + 1}";

                    return "Team ?";
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return "Team ?";
                }
            }
        }
        public TeamViewModel LowerRightTeam
        {
            get
            {
                int index = 3 + 4 * PageNumber;

                try
                {
                    if (GeneratedTeams[index] != null)
                        return GeneratedTeams[index];

                    return null;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    return null;
                }
            }
        }
        #endregion

        #region Commands
        public GoToMainMenuCommand GoToMainMenuCommand { get; set; } = new();

        public SelectAllPlayersInGeneratorViewCommand SelectAllPlayersInGeneratorViewCommand { get; set; } = new();
        public GenerateTeamsCommand GenerateTeamsCommand { get; set; } = new();

        public ShowPreviousTeamPageCommmand ShowPreviousTeamPageCommmand { get; set; } = new();
        public ShowNextTeamPageCommand ShowNextTeamPageCommand { get; set; } = new();
        #endregion

        #region OnChanged Events
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        private void OnGeneratedTeamsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(UpperLeftTeamName));
            OnPropertyChanged(nameof(UpperLeftTeam));

            OnPropertyChanged(nameof(UpperRightTeamName));
            OnPropertyChanged(nameof(UpperRightTeam));

            OnPropertyChanged(nameof(LowerLeftTeamName));
            OnPropertyChanged(nameof(LowerLeftTeam));

            OnPropertyChanged(nameof(LowerRightTeamName));
            OnPropertyChanged(nameof(LowerRightTeam));
        }
        #endregion

        public GenerateTeamsViewModel()
        {
            GeneratedTeams = new();
            GeneratedTeams.CollectionChanged += OnGeneratedTeamsChanged;

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

            LeftOverPlayers = new(RegisteredPlayers);

            foreach (TeamViewModel teamVM in GeneratedTeams)
                foreach (PlayerViewModel playerVM in teamVM.Players)
                    LeftOverPlayers.Remove(playerVM);
        }
    }
}
