using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGenerator.Commands;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.Models;
using System.ComponentModel;
using TeamGenerator.Enums;

namespace TeamGenerator.MVVM.ViewModels
{
    public class PlayerEditViewModel
    {
        public ObservableCollection<PlayerViewModel> RegistreredPlayers { get; set; } = new ObservableCollection<PlayerViewModel>();

        private PlayerViewModel _selectedPlayer;
        public PlayerViewModel SelectedPlayer { 
            get
            {
                return _selectedPlayer;
            }

            set
            {
                _selectedPlayer = value;

                if (_selectedPlayer.Languages.Contains(Language.Danish))
                    Danish = true;

                if (_selectedPlayer.Languages.Contains(Language.English))
                    English = true;
            }
        }

        private bool _danish;
        public bool Danish // does the player speak danish?
        {
            get
            {
                return _danish;
            }

            set
            {
                _danish = value;
                OnPropertyChanged(nameof(Danish));
            }
        }

        public bool English { get; set; } // does the player speak english?

        #region Commands
        public AddExclusionCommand AddExclusionCommand = new AddExclusionCommand();
        public RemoveExclusionCommand RemoveExclusionCommand = new RemoveExclusionCommand();

        public AddInclusionCommand AddInclusionCommand = new AddInclusionCommand();
        public RemoveInclusionCommand RemoveInclusionCommand = new RemoveInclusionCommand();
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;

            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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
