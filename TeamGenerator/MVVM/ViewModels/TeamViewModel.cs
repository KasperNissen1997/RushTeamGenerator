using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using TeamGenerator.MVVM.Models;

namespace TeamGenerator.MVVM.ViewModels
{
    public class TeamViewModel
    {
        private Team source;

        public ObservableCollection<PlayerViewModel> Players { get; }
        public int Rating
        {
            get
            {
                return source.Rating;
            }
        }
        public double AveragePlayerRating
        {
            get
            {
                return Math.Round(source.AveragePlayerRating, 1); 
            }
        }

        public bool SpeaksDanish
        {
            get
            {
                return source.SpeaksDanish;
            }
        }
        public bool SpeaksEnglish
        {
            get
            {
                return source.SpeaksEnglish;
            }
        }

        public TeamViewModel(Team source)
        {
            this.source = source;

            Players = new ObservableCollection<PlayerViewModel>();

            foreach (Player player in source.Players)
                Players.Add(new PlayerViewModel(player));
        }
    }
}
