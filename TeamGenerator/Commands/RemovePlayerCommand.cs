using System;
using System.Windows.Input;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.Commands
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
            if (parameter is PlayerEditViewModel vm)
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
            if (parameter is PlayerEditViewModel vm)
            {
                vm.RegisteredPlayers.Remove(vm.SelectedPlayer);
                vm.SelectedPlayer = null;
                return;
            }

            throw new InvalidOperationException("Something went horribly wrong!");
        }
    }
}
