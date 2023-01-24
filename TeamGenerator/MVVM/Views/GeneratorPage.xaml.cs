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
using TeamGenerator.MVVM.Models;

namespace TeamGenerator.MVVM.Views
{
    /// <summary>
    /// Interaction logic for GeneratorPage.xaml
    /// </summary>
    public partial class GeneratorPage : Page
    {
        public GeneratorPage()
        {
            InitializeComponent();
        }

        private void Gen_Team_Click(object sender, RoutedEventArgs e)
        {
            Generator generator = new Generator();
            //generator.TryGenerateTeams();
        }
    }
}
