using System;
using System.Collections.ObjectModel;
using TeamGenerator.Commands;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.Models;
using System.ComponentModel;

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

                if (SelectedPlayer is not null)
                {
                    SelectedPlayer.IsSelectedPlayer = true;

                    foreach (PlayerViewModel registeredPlayer in RegisteredPlayers)
                    {
                        registeredPlayer.IsInclusionOfSelectedPlayer = false;
                        registeredPlayer.IsExclusionOfSelectedPlayer = false;

                        if (SelectedPlayer.Inclusions.Contains(registeredPlayer))
                            registeredPlayer.IsInclusionOfSelectedPlayer = true;

                        if (SelectedPlayer.Exclusions.Contains(registeredPlayer))
                            registeredPlayer.IsExclusionOfSelectedPlayer = true;
                    }
                }
            }
        }

        public ObservableCollection<PlayerViewModel> InclusionsPlayerList { get; set; }
        public ObservableCollection<PlayerViewModel> ExclusionsPlayerList { get; set; }

        #region Commands
        public CreatePlayerCommand CreatePlayerCommand { get; set; } = new();
        public RemovePlayerCommand RemovePlayerCommand { get; set; } = new();

        public AddExclusionCommand AddExclusionCommand { get; set; } = new();
        public RemoveExclusionCommand RemoveExclusionCommand { get; set; } = new();

        public AddInclusionCommand AddInclusionCommand { get; set; } = new();
        public RemoveInclusionCommand RemoveInclusionCommand { get; set; } = new();
        #endregion

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
            RegisteredPlayers = new ObservableCollection<PlayerViewModel>();

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
        /// This method updates all the source referenes in each PlayerViewModel, so that the sources reflect the state of the ViewModels.
        /// This should be called before the repository saves the player data, to ensure the correct data is saved.
        /// </summary>
        public void UpdatePlayerViewModelSources()
        {
            foreach (PlayerViewModel registeredPlayer in RegisteredPlayers)
                registeredPlayer.Update();
        }
    }
}
