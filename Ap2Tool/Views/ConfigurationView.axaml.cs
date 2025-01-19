using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ap2Tool.ViewModels;

namespace Ap2Tool.Views;

public partial class ConfigurationView : UserControl
{
    // View가 화면에 표시될 때마다 생성자가 호출된다.

    public ConfigurationView()
    {
        InitializeComponent();
    }

    public ConfigurationView(ConfigurationViewModel vm) : this()
    {
        this.DataContext = vm;
    }

    ConfigurationViewModel? GetViewModel()
    {
        ConfigurationViewModel? vm = this.DataContext as ConfigurationViewModel;
        return vm;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ConfigurationViewModel? vm = GetViewModel();
        if (vm != null)
            vm.Initialize();
    }

    private void TreeView_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        var si = treeView.SelectedItem;
        if (si != null)
        {
            var vm = GetViewModel();
            vm.OnTreeViewSelectionChanged(treeView);
        }
    }

    private void CheckBox_Checked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = GetViewModel();
        vm.OnItemValueCheckBoxChecked(itemValueCheckBox.IsChecked ?? false);
    }

    private void TextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        string strText = itemValueTextBox.Text;
        var vm = GetViewModel();
        vm.OnItemValueTextTextChanged(strText);
    }

    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = GetViewModel();
        vm.OnSearchButtonClick(treeView, textSearch.Text);
    }
}