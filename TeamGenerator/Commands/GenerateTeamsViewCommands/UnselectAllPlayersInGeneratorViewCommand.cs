using System;
using System.Windows.Input;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.Commands.GenerateTeamsViewCommands
{
    public class UnselectAllPlayersInGeneratorViewCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is GenerateTeamsViewModel vm)
            {
                bool isAnyPlayerSelected = false;
                foreach (PlayerViewModel playerVM in vm.RegisteredPlayers)
                    if (playerVM.IsSelectedInTeamGeneratorView)
                        isAnyPlayerSelected = true;

                return isAnyPlayerSelected;
            }

            if (parameter is null) // in case this is called before the DataContext is created
                return false;

            throw new InvalidOperationException("Something went horribly wrong!");
        }

        public void Execute(object? parameter)
        {
            if (parameter is GenerateTeamsViewModel vm)
            {
                foreach (PlayerViewModel playerVM in vm.RegisteredPlayers)
                    playerVM.IsSelectedInTeamGeneratorView = false;

                return;
            }

            throw new InvalidOperationException("Something went horribly wrong!");
        }
    }
}
