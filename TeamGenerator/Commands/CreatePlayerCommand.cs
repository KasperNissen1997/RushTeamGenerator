using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.Models;
using TeamGenerator.MVVM.ViewModels;
using TeamGenerator.MVVM.Views;

namespace TeamGenerator.Commands
{
    public class CreatePlayerCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is EditPlayersViewModel vm)
            {
                CreatePlayerView createPlayerView = new CreatePlayerView();
                if (createPlayerView.ShowDialog() == true)
                {
                    CreatePlayerViewModel? dataContext = createPlayerView.DataContext as CreatePlayerViewModel;

                    string name = dataContext.Name;
                    string nickname = dataContext.Nickname;
                    int rating = dataContext.Rating;

                    bool speaksDanish = dataContext.SpeaksDanish;
                    bool speaksEnglish = dataContext.SpeaksEnglish;

                    Player player = PlayerRepository.Instance.Create(name, nickname, rating, speaksDanish, speaksEnglish);
                    PlayerViewModel playerVM = new PlayerViewModel(player);

                    vm.RegisteredPlayers.Add(playerVM);
                    vm.SelectedPlayer = playerVM;
                }

                return;
            }

            throw new InvalidOperationException("Something went horribly wrong!");
        }
    }
}
