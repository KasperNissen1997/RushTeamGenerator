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
using System.Windows.Shapes;

namespace TeamGenerator.MVVM.Views
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void Bnt1_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new GeneratorPage();
            Hide();
        }
        private void Bnt2_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new EditPlayersView();
            Hide();
        }
        public void Hide()
        {
            Bnt1.Visibility = Visibility.Hidden;
            Bnt2.Visibility = Visibility.Hidden;
            Logo.Visibility = Visibility.Hidden;
            Title.Visibility = Visibility.Hidden;
        }
    }
}
