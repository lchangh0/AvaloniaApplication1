using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test1
{
    public partial class MainWindowViewModel : ObservableObject
    {
        //public ObservableCollection<Country> Countries { get; }

        [ObservableProperty]
        public List<Country> countries;

        [ObservableProperty]
        private Country selectedCountry;

        public IRelayCommand<object> ComboBoxSelectionChangedCommand { get; }

        public MainWindowViewModel()
        {
            Countries = new List<Country>
            { 
                new Country { Name="South Korea", Flag = "태극기"  },
                new Country { Name="USA", Flag = "성조기"},
            };

            ComboBoxSelectionChangedCommand = new RelayCommand<object>(OnComboBoxSelectionChanged);
        }


        void OnComboBoxSelectionChanged(object? sender)
        {
            int dummy = 0;
        }

        [RelayCommand]
        public void ComboBox_SelectionChanged(SelectionChangedEventArgs e)
        {
            int dummy = 0;
        }


    }


    public class Country
    {
        public string Name { get; set; }
        public string Flag { get; set; }
    }

}
