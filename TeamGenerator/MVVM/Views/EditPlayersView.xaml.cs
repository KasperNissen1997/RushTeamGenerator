using System;
using System.Windows;
using System.Windows.Controls;
using TeamGenerator.MVVM.Models.Repositories;
using TeamGenerator.MVVM.ViewModels;

namespace TeamGenerator.MVVM.Views
{
    public partial class EditPlayersView : Page
    {
        public EditPlayersView()
        {
            InitializeComponent();

            DataContext = new EditPlayersViewModel();
        }

        private void Window_Closed(object sender, EventArgs e, EditPlayersViewModel DataContext)
        {
            if (DataContext is EditPlayersViewModel vm)
            {
                vm.UpdatePlayerViewModelSources();
                PlayerRepository.Instance.Save();
                return;
            }

            throw new InvalidOperationException("NO DATA CAN BE SAVED!");
        }
    }
}
