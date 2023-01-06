using System.Collections.ObjectModel;
using TeamGenerator.Enums;
using TeamGenerator.MVVM.Models;
using TeamGenerator.MVVM.Models.Repositories;

namespace TeamGenerator.MVVM.ViewModels
{
    public class PlayerViewModel
    {
        private Player source;

        public string Name { get; set; }
        public string Nickname { get; set; }
        public ObservableCollection<Language> Languages { get; set; }
        public int Rating { get; set; }

        public ObservableCollection<PlayerViewModel> Inclusions { get; }
        public ObservableCollection<PlayerViewModel> Exclusions { get; }

        public PlayerViewModel (Player source)
        {
            this.source = source;

            Name = source.Name;
            Nickname = source.Nickname;
            Languages = new ObservableCollection<Language>(source.Languages);
            Rating = source.Rating;

            Inclusions = new ObservableCollection<PlayerViewModel>();

            foreach (Player player in source.Inclusions)
                Inclusions.Add(new PlayerViewModel(player));

            Exclusions = new ObservableCollection<PlayerViewModel>();

            foreach (Player player in source.Inclusions)
                Exclusions.Add(new PlayerViewModel(player));
        }

        public void Delete ()
        {
            PlayerRepository.Instance.Delete(source.Identifier);
        }
    }
}
