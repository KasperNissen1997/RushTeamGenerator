using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.Models;
using TeamGenerator.MVVM.ViewModels;
using TeamGenerator.MVVM.Views;
using Microsoft.Win32;

namespace TeamGenerator.Commands.GenerateTeamsViewCommands
{
    public class ExportToCSVCommand : ICommand
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
                if (vm.GeneratedTeams.Count > 0)
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
                SaveFileDialog saveFileDialog = new();

                saveFileDialog.Filter = "CSV (Comma delimited)|*.csv";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (saveFileDialog.ShowDialog() == true)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine($"Team;Speaks Danish;Speaks English;Team Rating;Team Average;Player Rating;Player Nickname;Player Name;Player Speaks Danish;Player Speaks English");
                    sb.AppendLine();

                    for (int i = 0; i < vm.GeneratedTeams.Count; i++)
                    {
                        TeamViewModel teamVM = vm.GeneratedTeams[i];

                        sb.Append($"{i + 1};{teamVM.SpeaksDanish};{teamVM.SpeaksEnglish};{teamVM.Rating};{teamVM.AveragePlayerRating};");

                        for (int j = 0; j < teamVM.Players.Count; j++)
                        {
                            PlayerViewModel playerVM = vm.GeneratedTeams[i].Players[j];
                            if (j == 0)
                                sb.AppendLine($"{playerVM.Rating};{playerVM.Nickname};{playerVM.Name};{playerVM.SpeaksDanish};{playerVM.SpeaksEnglish}");
                            else
                                sb.AppendLine($";;;;;{playerVM.Rating};{playerVM.Nickname};{playerVM.Name};{playerVM.SpeaksDanish};{playerVM.SpeaksEnglish}");
                        }

                        sb.AppendLine();
                    }

                    sb.Append($"Left Over Players;;;;;");

                    for (int i = 0; i < vm.LeftOverPlayers.Count; i++)
                    {
                        PlayerViewModel playerVM = vm.LeftOverPlayers[i];

                        if (i == 0)
                            sb.AppendLine($"{playerVM.Rating};{playerVM.Nickname};{playerVM.Name};{playerVM.SpeaksDanish};{playerVM.SpeaksEnglish}");
                        else
                            sb.AppendLine($";;;;;{playerVM.Rating};{playerVM.Nickname};{playerVM.Name};{playerVM.SpeaksDanish};{playerVM.SpeaksEnglish}");
                    }

                    File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                }

                return;
            }
        }
    }
}
