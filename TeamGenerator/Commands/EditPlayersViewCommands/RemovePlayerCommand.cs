using System;
using System.Collections.Generic;
using System.Windows.Input;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.Commands.EditPlayersViewCommands
{
    public class RemovePlayerCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is EditPlayersViewModel vm)
            {
                if (vm.SelectedPlayer is not null)
                    return true;

                return false;
            }

            if (parameter is null) // in case this is called before the DataContext is created
                return false;

            throw new InvalidOperationException("Something went horribly wrong!");
        }

        public void Execute(object? parameter)
        {
            if (parameter is EditPlayersViewModel vm)
            {
                foreach (PlayerViewModel includedPlayerVM in vm.RegisteredPlayers) // remove relations before deletion
                    vm.SelectedPlayer.TryRemoveRelation(includedPlayerVM);

                vm.SelectedPlayer.Delete();

                vm.RegisteredPlayers.Remove(vm.SelectedPlayer);
                vm.SelectedPlayer = null;

                return;
            }

            throw new InvalidOperationException("Something went horribly wrong!");
        }
    }
}
