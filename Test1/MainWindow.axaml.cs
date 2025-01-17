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

            ComboBoxItems = new ObservableCollection<CComboBoxItem>() 
            { 
                new CComboBoxItem() { NameA = "item1", Value = "value1"},
                new CComboBoxItem() { NameA = "item2", Value = "value2"},
            };
        }

        public ObservableCollection<CComboBoxItem> ComboBoxItems { get; set; }

        public class CComboBoxItem
        {
            public string NameA { get; set; }
            public string Value { get; set; }
        }

        public string Var1 { get; set; } = "Hi";

        private void ComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            var si = comboBox.SelectedItem;
        }

    }
}