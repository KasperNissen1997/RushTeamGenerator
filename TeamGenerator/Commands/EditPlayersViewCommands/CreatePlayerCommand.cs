using System;
using System.Collections.Generic;
using System.Windows.Input;
using TeamGenerator.MVVM.Models;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.ViewModels;
using TeamGenerator.MVVM.Views;

namespace TeamGenerator.Commands.EditPlayersViewCommands
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
                    CreatePlayerViewModel? createPlayerVM = createPlayerView.DataContext as CreatePlayerViewModel;

                    string name = createPlayerVM.Name;
                    string nickname = createPlayerVM.Nickname;
                    int rating = createPlayerVM.Rating;

                    bool speaksDanish = createPlayerVM.SpeaksDanish;
                    bool speaksEnglish = createPlayerVM.SpeaksEnglish;

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
