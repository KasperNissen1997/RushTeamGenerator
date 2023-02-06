using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TeamGenerator.MVVM.Models;
using TeamGenerator.MVVM.Models.Repositories;

namespace TeamGenerator.MVVM.ViewModels
{
    public class PlayerViewModel : INotifyPropertyChanged
    {
        // TODO: FIX THIS ACCESS MODIFIER BREACH 
        public Player source;

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _nickname;
        public string Nickname
        {
            get
            {
                return _nickname;
            }

            set 
            { 
                _nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }
        private int _rating;
        public int Rating
        {
            get
            {
                return _rating;
            }

            set
            {
                _rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        private bool _speaksDanish;
        public bool SpeaksDanish
        {
            get
            {
                return _speaksDanish;
            }

            set
            {
                _speaksDanish = value;
                OnPropertyChanged(nameof(SpeaksDanish));
            }
        }
        private bool _speaksEnglish;
        public bool SpeaksEnglish
        {
            get
            {
                return _speaksEnglish;
            }

            set
            {
                _speaksEnglish = value;
                OnPropertyChanged(nameof(SpeaksEnglish));
            }
        }

        public ObservableCollection<PlayerViewModel> Inclusions { get; }
        public ObservableCollection<PlayerViewModel> Exclusions { get; }
        public ObservableCollection<PlayerViewModel> Acquaintences { get; }

        #region View related properties
        private bool _isSelectedPlayer;
        public bool IsSelectedPlayer
        {
            get
            {
                return _isSelectedPlayer;
            }

            set
            {
                _isSelectedPlayer = value;
                OnPropertyChanged(nameof(IsSelectedPlayer));
            }
        }

        private bool _isRelationOfSelectedPlayer;
        public bool IsRelationOfSelectedPlayer
        {
            get
            {
                return _isRelationOfSelectedPlayer;
            }

            set
            {
                _isRelationOfSelectedPlayer = value;
                OnPropertyChanged(nameof(IsRelationOfSelectedPlayer));
            }
        }

        private bool _isInclusionOfSelectedPlayer;
        public bool IsInclusionOfSelectedPlayer
        {
            get
            {
                return _isInclusionOfSelectedPlayer;
            }

            set
            {
                _isInclusionOfSelectedPlayer = value;
                OnPropertyChanged(nameof(IsInclusionOfSelectedPlayer));
            }
        }
        private bool _isExclusionOfSelectedPlayer;
        public bool IsExclusionOfSelectedPlayer
        {
            get
            {
                return _isExclusionOfSelectedPlayer;
            }

            set
            {
                _isExclusionOfSelectedPlayer = value;
                OnPropertyChanged(nameof(IsExclusionOfSelectedPlayer));
            }
        }
        private bool _isAcquaintenceOfSelectedPlayer;
        public bool IsAcquaintenceOfSelectedPlayer
        {
            get
            {
                return _isAcquaintenceOfSelectedPlayer;
            }

            set
            {
                _isAcquaintenceOfSelectedPlayer = value;
                OnPropertyChanged(nameof(IsAcquaintenceOfSelectedPlayer));
            }
        }

        private bool _isSelectedInTeamGeneratorView;
        public bool IsSelectedInTeamGeneratorView
        {
            get
            {
                return _isSelectedInTeamGeneratorView;
            }

            set
            {
                _isSelectedInTeamGeneratorView = value;
                OnPropertyChanged(nameof(IsSelectedInTeamGeneratorView));
            }
        }
        #endregion

        #region Interface
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged is not null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
        #endregion

        public PlayerViewModel (Player source)
        {
            this.source = source;

            Name = source.Name;
            Nickname = source.Nickname;
            Rating = source.Rating;

            SpeaksDanish = source.SpeaksDanish;
            SpeaksEnglish = source.SpeaksEnglish;

            /*
             *  Initialize, but don't populate relations, as this will create a circular dependency.
             *  
             *  The relations are populated in the EditPlayersViewModel, as it has all PlayerViewModels instantiated 
             *  in its RegisteredPlayers property.
             */

            Inclusions = new();
            Exclusions = new();
            Acquaintences = new();

            IsSelectedPlayer = false;
            IsInclusionOfSelectedPlayer = false;
            IsExclusionOfSelectedPlayer = false;
            IsAcquaintenceOfSelectedPlayer = false;

            IsSelectedInTeamGeneratorView = false;
        }

        #region Relation logic
        /// <summary>
        /// Tries to create an exisiting relations to <paramref name="playerVM"/>. Nothing happens if there are no relations to create.
        /// </summary>
        /// <param name="playerVM">The <see cref="PlayerViewModel"/> to create relations to.</param>
        public void TryCreateRelation(PlayerViewModel playerVM)
        {
            if (source.Inclusions.Contains(playerVM.source))
                AddInclusion(playerVM);

            if (source.Exclusions.Contains(playerVM.source))
                AddExclusion(playerVM);

            if (source.Acquaintences.Contains(playerVM.source))
                AddAcquaintence(playerVM);
        }

        /// <summary>
        /// Tries to remove an existing relation to <paramref name="playerVM"/>. Nothing happens if there are no relations to remove.
        /// </summary>
        /// <param name="playerVM">The <see cref="PlayerViewModel"/> to remove relations to.</param>
        public void TryRemoveRelation(PlayerViewModel playerVM)
        {
            if (Inclusions.Contains(playerVM))
                RemoveInclusion(playerVM);

            if (Exclusions.Contains(playerVM))
                RemoveExclusion(playerVM);

            if (Acquaintences.Contains(playerVM))
                RemoveAcquaintence(playerVM);
        }

        public void AddInclusion(PlayerViewModel playerVM)
        {
            if (Inclusions.Contains(playerVM))
                return; // can't add two of the same player to inclusions

            if (Exclusions.Contains(playerVM))
                throw new InvalidOperationException(); // can't add a player in exclusion to inclusion

            if (playerVM.Exclusions.Contains(this))
                throw new InvalidOperationException(); // can't add a player who excludes "this" to inclusions

            Inclusions.Add(playerVM);

            if (!playerVM.Inclusions.Contains(this))
                playerVM.AddInclusion(this);

            AddAcquaintence(playerVM);
        }

        public void RemoveInclusion(PlayerViewModel playerVM)
        {
            if (!Inclusions.Contains(playerVM))
                throw new ArgumentException(); // the player is not in the inclusions of "this"

            Inclusions.Remove(playerVM);

            if (playerVM.Inclusions.Contains(this))
                playerVM.RemoveInclusion(this);
        }

        public void AddExclusion(PlayerViewModel playerVM)
        {
            if (Exclusions.Contains(playerVM))
                return; // can't add two of the same player to exclusions

            if (Inclusions.Contains(playerVM))
                throw new InvalidOperationException(); // can't add a player in inclusion to exclusion

            if (playerVM.Inclusions.Contains(this))
                throw new InvalidOperationException(); // can't add a player who includes "this" to exclusions

            Exclusions.Add(playerVM);

            if (!playerVM.Exclusions.Contains(this))
                playerVM.AddExclusion(this);

            if (Acquaintences.Contains(playerVM))
                RemoveAcquaintence(playerVM);
        }

        public void RemoveExclusion(PlayerViewModel playerVM)
        {
            if (!Exclusions.Contains(playerVM))
                throw new ArgumentException(); // the player is not in the exclusions of "this"

            Exclusions.Remove(playerVM);

            if (playerVM.Exclusions.Contains(this))
                playerVM.RemoveExclusion(this);
        }

        public void AddAcquaintence(PlayerViewModel playerVM)
        {
            if (Acquaintences.Contains(playerVM)) // is the player already an acquaintence?
                return;

            if (Exclusions.Contains(playerVM)) // is the player an exclusion?
                throw new InvalidOperationException();

            if (playerVM.Exclusions.Contains(this)) // does the player exclude "this"?
                throw new InvalidOperationException();

            Acquaintences.Add(playerVM);

            if (!playerVM.Acquaintences.Contains(this)) // does the player have "this" as an acquaintence?
                playerVM.AddAcquaintence(this);
        }

        public void RemoveAcquaintence(PlayerViewModel playerVM)
        {
            if (!Acquaintences.Contains(playerVM)) // is the player an acquaintence?
                throw new ArgumentException();

            Acquaintences.Remove(playerVM);

            if (playerVM.Acquaintences.Contains(this)) // does the player have "this" as an acquaintence?
                playerVM.RemoveInclusion(this);
        }
        #endregion

        public void Update ()
        {
            if (!Name.Equals(source.Name))
                source.Name = Name;
            
            if (!Nickname.Equals(source.Nickname))
                source.Nickname = Nickname;
            
            if (Rating != source.Rating)
                source.Rating = Rating;

            if (SpeaksDanish != source.SpeaksDanish)
                source.SpeaksDanish = SpeaksDanish;

            if (SpeaksEnglish != source.SpeaksEnglish)
                source.SpeaksEnglish = SpeaksEnglish;

            #region Update relations
            foreach (Player includedPlayer in new List<Player>(source.Inclusions)) // remove all of the sources inclusions
                source.RemoveInclusion(includedPlayer);

            foreach (Player excludedPlayer in new List<Player>(source.Exclusions)) // remove all of the sources exclusions
                source.RemoveExclusion(excludedPlayer);

            foreach (Player acquaintedPlayer in new List<Player>(source.Acquaintences)) // remove all of the sources acquaintences
                source.RemoveAcquaintence(acquaintedPlayer);

            foreach (PlayerViewModel includedPlayerVM in Inclusions) // add all the new inclutions
                source.AddInclusion(includedPlayerVM.source);

            foreach (PlayerViewModel excludedPlayerVM in Exclusions) // add all the new exclusions
                source.AddExclusion(excludedPlayerVM.source);

            foreach (PlayerViewModel acquaintedPlayer in Acquaintences) // add all of the new acquaintences
                source.AddAcquaintence(acquaintedPlayer.source);
            #endregion
        }

        public void Delete ()
        {
            PlayerRepository.Instance.Delete(source.Identifier);
        }

        public override bool Equals(object? obj)
        {
            if (obj is PlayerViewModel otherVM)
                return source.Identifier == otherVM.source.Identifier;

            return false;
        }
    }
}
