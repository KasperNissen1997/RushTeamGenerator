using System.Windows;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.MVVM.Views
{
    public partial class CreatePlayerView : Window
    {
        public CreatePlayerView()
        {
            InitializeComponent();

            DataContext = new CreatePlayerViewModel();
        }

        private void CreatePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
