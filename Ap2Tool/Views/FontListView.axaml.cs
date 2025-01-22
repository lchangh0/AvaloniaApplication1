using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ap2Tool.ViewModels;
using Avalonia.Platform.Storage;
using Avalonia.Layout;
using Avalonia.Rendering;

namespace Ap2Tool.Views;

public partial class FontListView : UserControl
{
    public FontListView()
    {
        InitializeComponent();
    }

    public FontListView(FontListViewModel vm) : this()
    {
        this.DataContext = vm;
    }

    FontListViewModel GetViewModel()
    {
        return this.DataContext as FontListViewModel;
    }

}