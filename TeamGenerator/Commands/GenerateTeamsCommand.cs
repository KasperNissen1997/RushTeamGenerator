using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TeamGenerator.MVVM.Models;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.Commands
{
    public class GenerateTeamsCommand : ICommand
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
                bool atleastOnePlayerSelected = false;
                foreach (PlayerViewModel playerVM in vm.RegisteredPlayers)
                    if (playerVM.IsSelectedInTeamGeneratorView)
                    {
                        atleastOnePlayerSelected = true;
                        break;
                    }

                if (vm.TeamCapacity > 0 && vm.TeamCapacity < vm.RegisteredPlayers.Count
                    && vm.AllowedRatingDeviance >= 0
                    && atleastOnePlayerSelected)
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
                Generator teamGen = new();

                List<Player> selectedPlayers = new();
                foreach (PlayerViewModel playerVM in vm.RegisteredPlayers)
                {
                    if (playerVM.IsSelectedInTeamGeneratorView)
                        selectedPlayers.Add(playerVM.source);
                }

                teamGen.TryGenerateTeams(selectedPlayers, vm.TeamCapacity, vm.AllowedRatingDeviance, out List<Team> teams);

                foreach (Team team in teams)
                    vm.GeneratedTeams.Add(new TeamViewModel(team));
            }
        }
    }
}
