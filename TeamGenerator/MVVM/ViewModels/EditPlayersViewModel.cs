using System;
using System.Collections.ObjectModel;
using TeamGenerator.Commands;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.Models;
using System.ComponentModel;

namespace TeamGenerator.MVVM.ViewModels
{
    public class EditPlayersViewModel
    {
        public ObservableCollection<PlayerViewModel> RegisteredPlayers { get; set; } = new ObservableCollection<PlayerViewModel>();

        public PlayerViewModel SelectedPlayer { get; set; }

        #region Commands
        public CreatePlayerCommand CreatePlayerCommand { get; set; } = new();
        public RemovePlayerCommand RemovePlayerCommand { get; set; } = new();

        public AddExclusionCommand AddExclusionCommand { get; set; } = new();
        public RemoveExclusionCommand RemoveExclusionCommand { get; set; } = new();

        public AddInclusionCommand AddInclusionCommand { get; set; } = new();
        public RemoveInclusionCommand RemoveInclusionCommand { get; set; } = new();
        #endregion

        public EditPlayersViewModel()
        {
            foreach (Player player in PlayerRepository.Instance.RetrieveAll())
            {
                PlayerViewModel playerVM = new PlayerViewModel(player);
                RegisteredPlayers.Add(playerVM);
            }
        }

        public void UpdatePlayerViewModelSources()
        {
            foreach (PlayerViewModel registeredPlayer in RegisteredPlayers)
                registeredPlayer.Update();
        }
    }
}
