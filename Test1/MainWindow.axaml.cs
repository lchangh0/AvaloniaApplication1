using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Test1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var si = comboBox.SelectedItem;
        }

    }
}