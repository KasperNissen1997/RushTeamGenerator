using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TeamGenerator.MVVM.Models;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.MVVM.Views
{
    /// <summary>
    /// Interaction logic for ChoosePlayers.xaml
    /// </summary>
    public partial class ChoosePlayers : Window
    {
        public ChoosePlayers()
        {
            InitializeComponent();

            DataContext = new EditPlayersViewModel();
        }
        
        private void Gen_Show_Loaded(object sender, RoutedEventArgs e)
        {
            if (Nr_Teams.Text == "" || Min_Team_Value.Text == "" || Max_Team_Value.Text == "")
            {
                Gen_Show.IsEnabled = false;
            }
            else
            {
                Gen_Show.IsEnabled = true;
            }
        }
        public SelectionMode SelectionMode { get; set; }
        public Team teamOut = new Team(30);
        private void Gen_Show_Click(object sender, RoutedEventArgs e)
        {
            //Generator generator = new Generator();
            //generator.TryGenerateTeams();

        }
    }
}
