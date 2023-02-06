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

            if (parameter is EditPlayersViewModel vm)
            {
                vm.UpdatePlayerViewModelSources();
                PlayerRepository.Instance.Save();
            }

            if (parameter is GenerateTeamsViewModel)
            {
                // TODO: Save teams or any other edits made while in the GenerateTeamsView
            }
        }
    }
}
