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
        private Player source;

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
             *  Initialize, but don't populate, relations, as this will create a circular dependency.
             *  
             *  The relations are populated in the EditPlayersViewModel, as it has all PlayerViewModels instantiated 
             *  in its RegisteredPlayers property.
             */

            Inclusions = new ObservableCollection<PlayerViewModel>();
            Exclusions = new ObservableCollection<PlayerViewModel>();

            IsSelectedPlayer = false;
            IsInclusionOfSelectedPlayer = false;
            IsExclusionOfSelectedPlayer = false;
        }

        #region Relation logic
        /// <summary>
        /// Tries to create exisiting relations to <paramref name="playerVM"/>. Nothing happens if there are no relations to create.
        /// </summary>
        /// <param name="playerVM">The <see cref="PlayerViewModel"/> to create relations to.</param>
        public void TryCreateRelation(PlayerViewModel playerVM)
        {
            if (source.Inclusions.Contains(playerVM.source))
                AddInclusion(playerVM);

            if (source.Exclusions.Contains(playerVM.source))
                AddExclusion(playerVM);
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
            playerVM.AddInclusion(this);
        }

        public void RemoveInclusion(PlayerViewModel playerVM)
        {
            if (!Inclusions.Contains(playerVM))
                throw new ArgumentException(); // the player is not in the inclusions of "this"

            Inclusions.Remove(playerVM);
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
        }

        public void RemoveExclusion(PlayerViewModel playerVM)
        {
            if (!Exclusions.Contains(playerVM))
                throw new ArgumentException(); // the player is not in the exclusions of "this"

            Exclusions.Remove(playerVM);
        }
        #endregion

        public void Update ()
        {
            if (!Name.Equals(source.Name))
                PlayerRepository.Instance.UpdateName(source.Identifier, Name);
            
            if (!Nickname.Equals(source.Nickname))
                PlayerRepository.Instance.UpdateNickname(source.Identifier, Nickname);
            
            if (Rating != source.Rating)
                PlayerRepository.Instance.UpdateRating(source.Identifier, Rating);
            
            if (SpeaksDanish != source.SpeaksDanish || SpeaksEnglish != source.SpeaksEnglish)
                PlayerRepository.Instance.UpdateLanguages(source.Identifier, SpeaksDanish, SpeaksEnglish);

            foreach (Player includedPlayer in new List<Player>(source.Inclusions)) // remove all of the sources inclusions
                source.RemoveInclusion(includedPlayer);

            foreach (Player excludedPlayer in new List<Player>(source.Exclusions)) // remove all of the sources exclusions
                source.RemoveExclusion(excludedPlayer);

            foreach (PlayerViewModel includedPlayerVM in Inclusions) // add all the new inclutions
                source.AddInclusion(includedPlayerVM.source);

            foreach (PlayerViewModel excludedPlayerVM in Exclusions) // add all the new exclusions
                source.AddExclusion(excludedPlayerVM.source);
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
