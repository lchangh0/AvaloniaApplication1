using Ap2Tool.Views;
using ApCommon;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenLib;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace Ap2Tool.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; } = "Welcome to Avalonia!";

        ViewModelBase[] ViewModels;

        // [ObservableProperty] 특성을 설정하며 CurrentViewModel Property가 자동으로 만들어진다.
        [ObservableProperty]
        ViewModelBase _CurrentViewModel;

        [ObservableProperty]
        string _MenuTextSystem;
        [ObservableProperty]
        string _MenuTextConfiguration;
        [ObservableProperty]
        string _MenuTextExit;
        [ObservableProperty]
        string _MenuTextFunction;
        [ObservableProperty]
        string _MenuTextConvertIni;
        [ObservableProperty]
        string _MenuTextMakeConfigResourceFile;
        [ObservableProperty]
        string _MenuTextFontList;


        public MainWindowViewModel()
        {
            ViewModels = new ViewModelBase[] {
                new MainViewModel(),
                new ConfigurationViewModel(),
                new FontListViewModel(),
            };

            _CurrentViewModel = ViewModels[0];
            ApplyResourceText();
        }


        void ApplyResourceText()
        {
            _MenuTextSystem = CResource.GetString("System");
            _MenuTextConfiguration = CResource.GetString(CResource.IDS_CONFIG);
            _MenuTextExit = CResource.GetString(CResource.IDS_EXIT);
            _MenuTextFunction = CResource.GetString("Function");
            _MenuTextConvertIni = "AP.ini to config.json";
            _MenuTextMakeConfigResourceFile = "Make Config Resource File";
            _MenuTextFontList = "Font List";
        }


        [RelayCommand]
        public void HandleCommandMenuExit()
        {
            Environment.Exit(0);
        }

        // [RelayComman] 특성을 설정하면 View에서 Command를 사용해서 호출할 수 있다.
        [RelayCommand]
        public void HandleCommandMenuConfiguration()
        {
            // 주의) Observable Property를 사용해야 화면이 변경된다.
            CurrentViewModel = ViewModels[1];
        }

        [RelayCommand]
        public async void HandleCommandMenuConvertIni()
        {
            var view = GetView<MainWindow>();
            var topLevel = TopLevel.GetTopLevel(view);
            var options = new FilePickerOpenOptions
            {
                Title = "Open Ap.ini File",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("ini") { Patterns = ["*.ini"] },],
            };

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);

            if (files.Count > 0)
            {
                string strApIniFilePath = files[0].Path.AbsolutePath;
                string strDir = Path.GetDirectoryName(strApIniFilePath)!;
                string strConfigJsonFilePath = Path.Combine(strDir, "config.json." + DateTime.Now.ToString("yyyyMMddHHmmss"));
                CApIniToConfigJson convert = new CApIniToConfigJson();
                bool bOk = convert.ConvertApIniToConfigJson(strApIniFilePath, strConfigJsonFilePath,
                    out string strErrMsg);

                if (bOk)
                {
                    string strMsg = "Finish" + Environment.NewLine + Environment.NewLine + strConfigJsonFilePath;
                    await MessageBox.ShowAsync(strMsg, "Success");
                }
                else
                    await MessageBox.ShowAsync(strErrMsg, "Error", MessageBoxButtons.OK);
            }
        }

        [RelayCommand]
        public async void HandleCommandMenuMakeConfigResourceFile()
        {
            string strWorkDir = CSolutionGlobal.WorkDir;
            string strConfigFilePath = Path.Combine(strWorkDir, "config.json");

            if (!CConfigLib.LoadConfigFromJsonFile(strConfigFilePath,
                out CConfig config, out string strErrMsg))
            {
                await MessageBox.ShowAsync(strErrMsg);
                return;
            }

            string strConfigResourceFilePath = Path.Combine(strWorkDir, "config.resource.en.txt");
            if (!CConfigLib.MakeConfigResourceFile(config,
                strConfigResourceFilePath, out strErrMsg))
            {
                await MessageBox.ShowAsync(strErrMsg);
                return;
            }

            await MessageBox.ShowAsync($"{strConfigResourceFilePath}");
        }

        [RelayCommand]
        public void HandleCommandMenuFontList()
        {
            // 주의) Observable Property를 사용해야 화면이 변경된다.
            CurrentViewModel = ViewModels[2];
        }
    }
}
