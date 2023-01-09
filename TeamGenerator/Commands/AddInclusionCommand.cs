using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.Commands
{
    public class AddInclusionCommand : ICommand
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
                    if (!vm.SelectedRelatedPlayer.IsInclusionOfSelectedPlayer 
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
                vm.SelectedPlayer.AddInclusion(vm.SelectedRelatedPlayer);

                vm.SelectedRelatedPlayer.IsRelationOfSelectedPlayer = true;
                vm.SelectedRelatedPlayer.IsInclusionOfSelectedPlayer = true;

                return;
            }

            throw new InvalidOperationException("Something went horribly wrong!");
        }
    }
}
