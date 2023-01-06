using System.Windows;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.MVVM.Views
{
    public partial class PlayerEditView : Window
    {
        public PlayerEditView()
        {
            DataContext = new PlayerEditViewModel();

            InitializeComponent();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            PlayerRepository.Instance.Save();
        }
    }
}
