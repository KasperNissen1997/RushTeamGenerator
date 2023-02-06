using System.Windows.Controls;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.MVVM.Views
{
    /// <summary>
    /// Interaction logic for GenerateTeamsView.xaml
    /// </summary>
    public partial class GenerateTeamsView : Page
    {
        public GenerateTeamsView()
        {
            InitializeComponent();

            DataContext = new GenerateTeamsViewModel();
        }
    }
}
