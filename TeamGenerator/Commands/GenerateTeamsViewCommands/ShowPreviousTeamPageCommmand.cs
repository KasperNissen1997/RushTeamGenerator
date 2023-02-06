using System;
using System.Windows.Input;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.Commands.GenerateTeamsViewCommands
{
    public class ShowPreviousTeamPageCommmand : ICommand
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
                if (vm.PageNumber > 0)
                    return true;

                return false;
            }

            if (parameter is null) // in case this is called before the DataContext is created
                return false;

            throw new InvalidOperationException("Something went horribly wrong!");
        }

        public void Execute(object? parameter)
        {
            if (parameter is GenerateTeamsViewModel vm)
            {
                vm.PageNumber--;
            }
        }
    }
}
