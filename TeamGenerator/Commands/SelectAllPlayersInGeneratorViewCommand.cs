using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.Commands
{
    public class SelectAllPlayersInGeneratorViewCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is GenerateTeamsViewModel vm)
            {
                foreach (PlayerViewModel playerVM in vm.RegisteredPlayers)
                    playerVM.IsSelectedInTeamGeneratorView = true;
                
                return;
            }

            throw new InvalidOperationException("Something went horribly wrong!");
        }
    }
}
