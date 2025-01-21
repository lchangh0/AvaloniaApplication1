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

    private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ChangeDataGridHeight();
    }

    private void UserControl_SizeChanged(object? sender, Avalonia.Controls.SizeChangedEventArgs e)
    {
        ChangeDataGridHeight();
    }

    void ChangeDataGridHeight()
    {
        //double d0 = gridUserControl.Height;
        //double d1 = gridContent.Height;
        //double d2 = stackPanelTop.Height;
        //double dHeight = stackPanelGrid.Height;

        Grid gridUserControl = this.FindControl<Grid>("gridUserControl");
        double d1 = gridUserControl.Height;
        
        //dataGrid.Height = 200;
    }

    private void UserControl_LayoutUpdated(object? sender, System.EventArgs e)
    {
        ChangeDataGridHeight();
    }
}