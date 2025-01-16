using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace Ap2Tool.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; } = "Welcome to Avalonia!";

        ViewModelBase[] ViewModels = new ViewModelBase[] {
            new MainViewModel(),
            new ConfigurationViewModel(),
            new ConvertIniViewModel(),
        };

        // [ObservableProperty] 특성을 설정하며 CurrentViewModel Property가 자동으로 만들어진다.
        [ObservableProperty]
        ViewModelBase _CurrentViewModel;
       
        public MainWindowViewModel()
        {
            _CurrentViewModel = ViewModels[0];
        }

        [RelayCommand]
        public void ExitProgram()
        {
            Environment.Exit(0);
        }

        // [RelayComman] 특성을 설정하면 View에서 Command를 사용해서 호출할 수 있다.
        [RelayCommand]
        public void GotoConfigurationView()
        {
            // 주의) Observable Property를 사용해야 화면이 변경된다.
            CurrentViewModel = ViewModels[1];
        }

        [RelayCommand]
        public void GotoConvertIniView()
        {
            // 주의) Observable Property를 사용해야 화면이 변경된다.
            CurrentViewModel = ViewModels[2];
        }

    }
}
