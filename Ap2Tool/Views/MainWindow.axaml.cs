using Ap2Tool.ViewModels;
using Avalonia.Controls;

namespace Ap2Tool.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainWindowViewModel();
            vm.SetView(this);
            this.DataContext = vm;
        }


    }
}