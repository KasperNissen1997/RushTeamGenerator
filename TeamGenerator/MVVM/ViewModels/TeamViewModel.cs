using System.Collections.ObjectModel;
using TeamGenerator.MVVM.Models;

namespace TeamGenerator.MVVM.ViewModels
{
    public class TeamViewModel
    {
        private Team source;

        public ObservableCollection<PlayerViewModel> Players { get; }
        public bool SpeaksDanish { get; }
        public bool SpeaksEnglish { get; }
        public float Rating { get; }

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
