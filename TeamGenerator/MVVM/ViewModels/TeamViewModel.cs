using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using TeamGenerator.MVVM.Models;

namespace TeamGenerator.MVVM.ViewModels
{
    public class TeamViewModel
    {
        private Team source;

        public ObservableCollection<PlayerViewModel> Players { get; set; }
        public bool SpeaksDanish { get; set; }
        public bool SpeaksEnglish { get; set; }
        public float Rating { get; set; }

        public TeamViewModel (Team source)
        {
            this.source = source;

            Players = new ObservableCollection<PlayerViewModel>();

            foreach (Player player in source.Players)
                Players.Add(new PlayerViewModel(player));

            Rating = source.Rating;

            SpeaksDanish = source.SpeaksDanish;
            SpeaksEnglish = source.SpeaksEnglish;
        }
    }
}
