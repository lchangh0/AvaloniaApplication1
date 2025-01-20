using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ap2Tool.ViewModels;
using System;
using Avalonia.Media.Imaging;
using System.IO;
using ApCommon;

namespace Ap2Tool.Views;

public partial class ConfigurationView : UserControl
{
    // View�� ȭ�鿡 ǥ�õ� ������ �����ڰ� ȣ��ȴ�.

    public ConfigurationView()
    {
        InitializeComponent();

        // ����) �� �����ڿ� ����� �߰��ϸ� �����̳� ȭ�鿡 ������ �߻��Ѵ�. ??
    }

    public ConfigurationView(ConfigurationViewModel vm) : this()
    {
        this.DataContext = vm;
        LoadImage();
    }

    void LoadImage()
    {
        string strFilePath = Path.Combine(CSolutionGlobal.WorkDir, "Assets", "gear.png");
        if (File.Exists(strFilePath))
        {
            var bitmap = new Bitmap(strFilePath);

            var imageControl = this.FindControl<Image>("imageConfig");
            if (imageControl != null)
                imageControl.Source = bitmap;
        }
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