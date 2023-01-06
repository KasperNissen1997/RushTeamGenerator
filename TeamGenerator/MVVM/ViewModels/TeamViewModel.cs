using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGenerator.Enums;
using TeamGenerator.MVVM.Models;

namespace TeamGenerator.MVVM.ViewModels
{
    public class TeamViewModel
    {
        private Team source;

        public ObservableCollection<PlayerViewModel> Players { get; set; }
        public ObservableCollection<Language> Languages { get; set; }
        public float Rating { get; set; }

        public TeamViewModel (Team source)
        {
            this.source = source;

            //Players = source.Players;
            //Languages = source.Languages;
            //Rating = source.Rating;
        }
    }
}
