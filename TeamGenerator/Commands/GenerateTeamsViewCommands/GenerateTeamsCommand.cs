using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TeamGenerator.MVVM.Models;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.Commands.GenerateTeamsViewCommands
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

                if (vm.TeamCapacity > 1 && vm.TeamCapacity < vm.RegisteredPlayers.Count
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
                List<Player> selectedPlayers = new();
                foreach (PlayerViewModel playerVM in vm.RegisteredPlayers)
                {
                    if (playerVM.IsSelectedInTeamGeneratorView)
                        selectedPlayers.Add(playerVM.source);
                }

                foreach (Team team in new List<Team>(TeamRepository.Instance.RetrieveAll()))
                    TeamRepository.Instance.Delete(team.Identifier);

                vm.GeneratedTeams.Clear();
                vm.LeftOverPlayers.Clear();
                vm.PageNumber = 0;

                Generator.GenerationResults results = Generator.TryGenerateTeams(selectedPlayers, vm.TeamCapacity, vm.AllowedRatingDeviance, out List<Team> teams, vm.OptimizationIterations);

                List<Player> playersCopy = new List<Player>(PlayerRepository.Instance.RetrieveAll());
                foreach (Team team in teams)
                {
                    vm.GeneratedTeams.Add(new TeamViewModel(team));

                    foreach (Player player in team.Players)
                        playersCopy.Remove(player);
                }

                foreach (Player leftOverPlayer in playersCopy)
                    vm.LeftOverPlayers.Add(new PlayerViewModel(leftOverPlayer));

                if (results.success)
                    MessageBox.Show(results.ToString(), "Team Generation Result", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                else
                    MessageBox.Show(results.ToString(), "Team Generation Result", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
            }
        }
    }
}
