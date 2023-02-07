using System;
using System.Windows.Input;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.ViewModels;
using TeamGenerator.MVVM.Views;

namespace TeamGenerator.Commands
{
    public class GoToMainMenuCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            MainWindow.Instance.MainFrame.Content = MainWindow.Instance.MainMenuView;
            MainWindow.Instance.Title = MainWindow.Instance.MainMenuView.Title;

            if (parameter is EditPlayersViewModel editPlayersVM)
            {
                editPlayersVM.UpdatePlayerViewModelSources();
                PlayerRepository.Instance.Save();
            }

            if (parameter is GenerateTeamsViewModel generateTeamsVM)
            {
                TeamRepository.Instance.Save();
            }
        }
    }
}
