using System;
using System.Windows.Input;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.Commands.EditPlayersViewCommands
{
    public class RemoveInclusionCommand : ICommand
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
                if (vm.SelectedRelatedPlayer is not null)
                    if (vm.SelectedRelatedPlayer.IsInclusionOfSelectedPlayer
                        && !vm.SelectedRelatedPlayer.IsExclusionOfSelectedPlayer
                        && !vm.SelectedPlayer.Equals(vm.SelectedRelatedPlayer))
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
                vm.SelectedPlayer.RemoveInclusion(vm.SelectedRelatedPlayer);

                vm.SelectedRelatedPlayer.IsRelationOfSelectedPlayer = false;
                vm.SelectedRelatedPlayer.IsInclusionOfSelectedPlayer = false;

                vm.SelectedRelatedPlayer = null;

                vm.RelationActionLog = "Exclusion removed succesfully.";

                return;
            }

            throw new InvalidOperationException("Something went horribly wrong!");
        }
    }
}
