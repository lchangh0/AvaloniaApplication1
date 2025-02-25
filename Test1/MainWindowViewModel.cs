﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
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
        public ObservableCollection<Country> countries;

        [ObservableProperty]
        bool _Option1Checked;

        [ObservableProperty]
        bool _Option2Checked;
        
        private Country selectedCountry;
        public Country SelectedCountry
        {
            get => selectedCountry;
            set
            {
                SetProperty(ref selectedCountry, value);
                OnSelectionChanged(value);
            }
        }

        void OnSelectionChanged(Country country)
        {
            int dummy = 0;
        }

        public MainWindowViewModel()
        {
            Countries = new ObservableCollection<Country>
            { 
                new Country { Name="South Korea", Flag = "태극기"  },
                new Country { Name="USA", Flag = "성조기"},
            };
        }


        [RelayCommand]
        public async void OnButtonClick()
        {
            Countries.Clear();
            Countries.Add(new Country { Name = "A" });
            Countries.Add(new Country { Name = "B" });

            var box = MessageBoxManager.GetMessageBoxStandard("Caption", "Test Message", MsBox.Avalonia.Enums.ButtonEnum.Ok);
            var result = await box.ShowAsync();
        }

        [RelayCommand]
        public void OnPrevButtonClick()
        {
            SelectedCountry = Countries[0];
        }

        [RelayCommand]
        public void OnNextButtonClick()
        {
            SelectedCountry = Countries[1];
        }

    }


    public class Country
    {
        public string Name { get; set; }
        public string Flag { get; set; }
    }

}
