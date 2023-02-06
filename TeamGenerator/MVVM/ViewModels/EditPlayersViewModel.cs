using System.Collections.ObjectModel;
using TeamGenerator.Commands;
using TeamGenerator.MVVM.Models;
using TeamGenerator.MVVM.Models.Repositories;
using System.ComponentModel;
using System.Windows.Controls;
using System;
using System.Diagnostics;

namespace TeamGenerator.MVVM.ViewModels
{
    public class EditPlayersViewModel : INotifyPropertyChanged
    {
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
                if (SelectedPlayer is not null)
                    SelectedPlayer.IsSelectedPlayer = false;

                _selectedPlayer = value;
                OnPropertyChanged(nameof(SelectedPlayer));

                foreach (PlayerViewModel registeredPlayerVM in RegisteredPlayers)
                {
                    registeredPlayerVM.IsRelationOfSelectedPlayer = false;
                    registeredPlayerVM.IsInclusionOfSelectedPlayer = false;
                    registeredPlayerVM.IsExclusionOfSelectedPlayer = false;
                    registeredPlayerVM.IsAcquaintenceOfSelectedPlayer = false;
                }

                if (SelectedPlayer is not null)
                {
                    SelectedPlayer.IsSelectedPlayer = true;

                    foreach (PlayerViewModel registeredPlayerVM in RegisteredPlayers)
                    {
                        if (SelectedPlayer.Inclusions.Contains(registeredPlayerVM))
                        {
                            registeredPlayerVM.IsRelationOfSelectedPlayer = true;
                            registeredPlayerVM.IsInclusionOfSelectedPlayer = true;
                        }

                        if (SelectedPlayer.Exclusions.Contains(registeredPlayerVM))
                        {
                            registeredPlayerVM.IsRelationOfSelectedPlayer = true;
                            registeredPlayerVM.IsExclusionOfSelectedPlayer = true;
                        }

                        if (SelectedPlayer.Acquaintences.Contains(registeredPlayerVM))
                        {
                            registeredPlayerVM.IsRelationOfSelectedPlayer = true;
                            registeredPlayerVM.IsAcquaintenceOfSelectedPlayer = true;
                        }
                    }
                }
            }
        }
        private PlayerViewModel _selectedRelatedPlayer;
        public PlayerViewModel SelectedRelatedPlayer
        {
            get
            {
                return _selectedRelatedPlayer;
            }

            set
            {
                _selectedRelatedPlayer = value;
                OnPropertyChanged(nameof(SelectedRelatedPlayer));
            }
        }

        #region Commands
        public GoToMainMenuCommand GoToMainMenuCommand { get; set; } = new();

        public CreatePlayerCommand CreatePlayerCommand { get; set; } = new();
        public RemovePlayerCommand RemovePlayerCommand { get; set; } = new();

        public AddInclusionCommand AddInclusionCommand { get; set; } = new();
        public RemoveInclusionCommand RemoveInclusionCommand { get; set; } = new();
        public AddExclusionCommand AddExclusionCommand { get; set; } = new();
        public RemoveExclusionCommand RemoveExclusionCommand { get; set; } = new();
        #endregion

        private string _relationActionLog = "Performed actions are logged here.";
        public string RelationActionLog
        {
            get
            {
                return _relationActionLog;
            }

            set
            {
                _relationActionLog = value;
                OnPropertyChanged(nameof(RelationActionLog));
            }
        }

        #region Interface
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged is not null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
        #endregion

        public EditPlayersViewModel()
        {
            RegisteredPlayers = new();

            foreach (Player player in PlayerRepository.Instance.RetrieveAll())
            {
                PlayerViewModel playerVM = new PlayerViewModel(player);
                RegisteredPlayers.Add(playerVM);
            }

            foreach (PlayerViewModel playerVM in RegisteredPlayers) // first iteration of all registered players
                foreach (PlayerViewModel potentialRelation in RegisteredPlayers) // second iteration
                    playerVM.TryCreateRelation(potentialRelation); // create relations, if any exist
        }

        /// <summary>
        /// This method updates all the source references in each PlayerViewModel, so that the sources reflect the state of the ViewModels.
        /// This should be called before the repository saves the player data, to ensure the correct data is saved.
        /// </summary>
        public void UpdatePlayerViewModelSources()
        {
            foreach (PlayerViewModel registeredPlayer in RegisteredPlayers)
                registeredPlayer.Update();

            Trace.WriteLine("PlayerViewModel sources updated!");
        }
    }
}
