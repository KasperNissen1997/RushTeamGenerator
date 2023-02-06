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
    }
}
