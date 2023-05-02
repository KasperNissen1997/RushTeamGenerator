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
using System.Windows.Navigation;
using System.IO;

namespace TeamGenerator.MVVM.Views
{
    /// <summary>
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView : Page
    {
        public MainMenuView()
        {
            InitializeComponent();

            pathLabel.Content = Path.GetFullPath(@"Data\Players.xml");
        }

        private void GenerateTeams_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.GenerateTeamsView = new GenerateTeamsView();
            MainWindow.Instance.MainFrame.Content = MainWindow.Instance.GenerateTeamsView;
            MainWindow.Instance.Title = MainWindow.Instance.GenerateTeamsView.Title;
        }

        private void EditPlayers_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.EditPlayersView = new EditPlayersView();
            MainWindow.Instance.MainFrame.Content = MainWindow.Instance.EditPlayersView;
            MainWindow.Instance.Title = MainWindow.Instance.EditPlayersView.Title;
        }
    }
}
