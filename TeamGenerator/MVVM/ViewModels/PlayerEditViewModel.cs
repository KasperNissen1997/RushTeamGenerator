using System;
using System.Collections.ObjectModel;
using TeamGenerator.Commands;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.Models;
using System.ComponentModel;

namespace TeamGenerator.MVVM.ViewModels
{
    public class PlayerEditViewModel
    {
        public ObservableCollection<PlayerViewModel> RegistreredPlayers { get; set; } = new ObservableCollection<PlayerViewModel>();

        public PlayerViewModel SelectedPlayer { get; set; }

        #region Commands
        public AddExclusionCommand AddExclusionCommand = new AddExclusionCommand();
        public RemoveExclusionCommand RemoveExclusionCommand = new RemoveExclusionCommand();

        public AddInclusionCommand AddInclusionCommand = new AddInclusionCommand();
        public RemoveInclusionCommand RemoveInclusionCommand = new RemoveInclusionCommand();
        #endregion

        public PlayerEditViewModel ()
        {
            foreach (Player player in PlayerRepository.Instance.RetrieveAll())
            {
                PlayerViewModel playerVM = new PlayerViewModel(player);
                RegistreredPlayers.Add(playerVM);
            }
        }
    }
}
