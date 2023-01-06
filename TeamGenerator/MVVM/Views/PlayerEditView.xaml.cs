using System.Windows;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator
{
    /// <summary>
    /// Interaction logic for PlayerEditView.xaml
    /// </summary>
    public partial class PlayerEditView : Window
    {
        public PlayerEditView()
        {
            DataContext = new PlayerEditViewModel();

            InitializeComponent();
        }
    }
}
