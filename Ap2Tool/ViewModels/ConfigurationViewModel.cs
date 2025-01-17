using Ap2Tool.Models;
using ApCommon;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentFTP;
using GenLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ap2Tool.ViewModels
{
    public partial class ConfigurationViewModel : ViewModelBase
    {
        public string Title { get; set; } = "Configuration";


        [ObservableProperty]
        CTreeNode _TreeRoot;

        string m_strLogFolderPath;
        string m_strConfigFilePath;
        string m_strDefaultConfigFilePath;
        string m_strCofigJsonLoaded;
        string m_lang = "en";
        CConfig m_config;
        CConfig m_configLoaded;

        public ConfigurationViewModel()
        {
            _TreeRoot = new CTreeNode("");

            ItemDescriptionTitle = "Description";
        }


        [RelayCommand]
        public async void Search(string strCondition)
        {
            int dummy = 0;
        }


        public async Task<bool> InitializeAsync()
        {
            if (ConfigurationManager.AppSettings.Count == 0)
            {
                AppConfig.CreateAppConfigFile();
            }

            m_strDefaultConfigFilePath = Path.Combine(CSolutionGlobal.WorkDir, CConst.CONFIG_FILE);
            m_strLogFolderPath = CSolutionGlobal.LogDir;

            ClearItemDisplay();

            if (!await LoadAndInitConfigAsync(m_strDefaultConfigFilePath))
                return false;

            if (!InitTreeviewAndShow())
                return false;

            return true;
        }


        async Task<bool> LoadAndInitConfigAsync(string strConfigFilePath)
        {
            m_strConfigFilePath = strConfigFilePath;
            m_config = new CConfig();
            StringBuilder sbLog = new StringBuilder();
            m_config.BuildTree(sbLog);

            if (File.Exists(strConfigFilePath) == false)
            {
                string strMsg = CResource.GetString(CResource.IDS_MSG_CONFIG_FILE_NOT_FOUND_CONTINUE,
                    "Config file is not found.\r\nDo you want to proceed with defaults?");
                string strCaption = CResource.GetString(CResource.IDS_ERROR, "Error");

                if (await MessageBox.ShowAsync(strMsg, strCaption, MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return false;
                }
                m_strCofigJsonLoaded = CJson.GetJsonStr(m_config);
                await SaveConfigJsonAsync();
            }
            else
            {
                string strJson = CGenLib.ReadFileAllText(strConfigFilePath);

                CConfig configFile = CJson.ParseJsonStr<CConfig>(strJson);

                if (configFile == null)
                {
                    await MessageBox.ShowAsync("Json File Load Failure");
                    return false;
                }

                StringBuilder sbLog2 = new StringBuilder();
                configFile.RebuildNodeDictionary(sbLog2);
                configFile.AdjustForConsistency();
                m_config.UpdateValueAndComment(configFile);
                m_strCofigJsonLoaded = CJson.GetJsonStr(m_config);
            }

            m_configLoaded = new CConfig(m_config);

            // 리소스파일을 로드한다.
            string strResourceFileName = m_config.GetValue<string>(EConfigId.ResourceFileName);
            LoadResourceFile(strResourceFileName, m_config);
            m_lang = CResource.Lang;

            return true;
        }


        async Task<bool> SaveConfigJsonAsync(bool bNeedCompareToPopup = false)
        {
            string strConfigJson = CJson.GetJsonStr(m_config);

            if (bNeedCompareToPopup)
            {
                if (strConfigJson == m_strCofigJsonLoaded)
                {
                    return false;
                }
                else
                {
                    if (string.IsNullOrEmpty(m_strCofigJsonLoaded) != true)
                    {
                        string strMsg = CResource.GetString(CResource.IDS_MSG_SAVE_MODIFICATION_CONFIRM,
                            "A modification has taken place.\r\nDo you want to save the changes?");
                        string strCaption = CResource.GetString(CResource.IDS_WARNING, "Warning");

                        if (await MessageBox.ShowAsync(strMsg, strCaption, MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            return false;
                        }
                    }
                }
            }

            if (m_config != null)
            {
                if (m_strConfigFilePath == m_strDefaultConfigFilePath)
                {
                    // 저장하기 전 파일 백업

                    if (Directory.Exists(m_strLogFolderPath) == false)
                        Directory.CreateDirectory(m_strLogFolderPath);

                    if (strConfigJson != m_strCofigJsonLoaded)
                    {
                        string DatetimeNow = DateTime.Now.ToString("yyyyMMddhhmmss");
                        string strConfigFilePathBack = Path.Combine(m_strLogFolderPath, "config.json." + DatetimeNow + ".e.txt");

                        if (CGenLib.WriteFileAllText(strConfigFilePathBack, m_strCofigJsonLoaded, Encoding.UTF8, true) == false)
                        {
                            await MessageBox.ShowAsync("Backup Fail", "Error");
                        }
                    }
                }

                // 파일변경시간 및 체크섬을 설정
                m_config.SetFileChanged();
                strConfigJson = CJson.GetJsonStr(m_config);

                if (CGenLib.WriteFileAllText(m_strConfigFilePath, strConfigJson, Encoding.UTF8, bFlushToDisk: true) == false)
                {
                    await MessageBox.ShowAsync("Config.json Write Fail", "Error");
                    return false;
                }
                m_strCofigJsonLoaded = strConfigJson;

                return true;
            }
            return false;
        }


        public bool LoadResourceFile(string strFileName, CConfig config)
        {
            string strResourceFile = Path.Combine(CSolutionGlobal.WorkDir, strFileName);
            if (!CResource.LoadResourceFile(strResourceFile))
                return false;

            string strConfigResourceFilePath = Path.Combine(CSolutionGlobal.WorkDir, $"config.resource.{CResource.Lang}.txt");
            if (CConfigLib.LoadConfigResourceFile(strConfigResourceFilePath,
                out Dictionary<string, string> dictResource,
                out string strErrMsg))
                config.UpdateResource(dictResource);
            else
                config.ClearResource();

            return true;
        }


        bool InitTreeviewAndShow()
        {
            LoadConfigToTreeView(m_config, TreeRoot);
            return true;
        }


        List<string> m_listSkipConfigIds;

        public void LoadConfigToTreeView(CConfig config, CTreeNode treeRoot)
        {
            if (config == null)
                return;

            if (treeRoot.SubNodes.Count > 0)
                treeRoot.SubNodes.Clear();

            m_listSkipConfigIds = GetSkipConfigIds();

            AddConfigNodes(config.Root, treeRoot);
        }

        
        void AddConfigNodes(CConfigNode configNode, CTreeNode parent)
        {
            if (m_listSkipConfigIds.Contains(configNode.Id.ToString()))
                return;

            CTreeNode treeNode;

            if (configNode.Id == EConfigId.Root)
            {
                treeNode = parent;
            }
            else
            {
                string strText = configNode.GetTitle(m_lang == "ko");
                treeNode = new CTreeNode(strText, configNode);
                //trN.Name = configNode.Id.ToString();

                parent.SubNodes.Add(treeNode);
            }

            foreach (var childNode in configNode.ChildNodes)
            {
                // 재귀
                AddConfigNodes(childNode, treeNode);
            }
        }


        static List<string> GetSkipConfigIds()
        {
            List<string> list = new List<string>();

            if (CGenLib.StrToBool(AppConfig.GetValue("ShowDevelopGroup", "false")) == false)
                list.Add("Development");

            if (CGenLib.StrToBool(AppConfig.GetValue("ShowUiGroup", "false")) == false)
                list.Add("UI");

            return list;
        }

        public class CComboBoxItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        //[ObservableProperty]
        //CConfigNode _SelectedConfigItem;

        [ObservableProperty]
        string _ItemTitle;

        [ObservableProperty]
        string _ItemValueText;

        [ObservableProperty]
        bool _ItemValueBool;

        [ObservableProperty]
        string _ItemValueBoolTitle;

        //[ObservableProperty]
        //List<CComboBoxItem> _ItemValueComboBoxItems = new List<CComboBoxItem>();
        ObservableCollection<CComboBoxItem> ItemValueComboBoxItems { get; }
         = new ObservableCollection<CComboBoxItem>();

        [ObservableProperty]
        bool _ItemValueTextVisible;

        [ObservableProperty]
        bool _ItemValueBoolVisible;

        [ObservableProperty]
        bool _ItemValueEnumVisible;


        [ObservableProperty]
        string _ItemDescriptionTitle;

        [ObservableProperty]
        string _ItemDescription;


        CConfigNode _SelectedConfigNode;

        public void OnTreeViewSelectionChanged(TreeView treeView)
        {
            ClearItemDisplay();

            var si = treeView.SelectedItem as CTreeNode;
            if (si != null)
            {
                var configNode = si.Value as CConfigNode;
                if (configNode != null)
                {
                    _SelectedConfigNode = configNode;

                    ItemTitle = configNode.GetTitle(m_lang == "ko");

                    if (configNode.NodeType == CConfigNode.ENodeType.Item)
                    {
                        ItemValueText = configNode.Value;

                        if (configNode.ValueType == CConfigNode.EValueType.Bool)
                        {
                            ItemValueBoolVisible = true;
                            ItemValueBool = CGenLib.StrToBool(configNode.Value);
                            ItemValueBoolTitle = "Set";
                        }
                        else if (configNode.ValueType == CConfigNode.EValueType.Enum)
                        {
                            ItemValueComboBoxItems.Clear();
                            ItemValueComboBoxItems.Add(new CComboBoxItem() { Name = "name1", Value = "value1" });
                            ItemValueComboBoxItems.Add(new CComboBoxItem() { Name = "name2", Value = "value2" });
                            ItemValueEnumVisible = true;
                        }
                        else
                            ItemValueTextVisible = true;
                    }

                    ItemDescription = configNode.GetDescription(m_lang == "ko");
                }
            }
        }

        void ClearItemDisplay()
        {
            _SelectedConfigNode = null;

            ItemTitle = string.Empty;
            ItemValueText = string.Empty;
            ItemValueBool = false;
            ItemValueBoolTitle = "";
            ItemValueComboBoxItems.Clear();
            ItemValueTextVisible = false;
            ItemValueBoolVisible = false;
            ItemValueEnumVisible = false;
            ItemDescription = string.Empty;
        }

        public void OnItemValueCheckBoxChecked(bool bChecked)
        {
            _ItemValueBool = bChecked;

            if (_SelectedConfigNode != null)
            {
                _SelectedConfigNode.Value = bChecked.ToString();
            }
        }

        public void OnItemValueTextTextChanged(string strText)
        {
            _ItemValueText = strText;
            if (_SelectedConfigNode != null)
            {
                if (_SelectedConfigNode.Value != strText)
                    _SelectedConfigNode.Value = strText;
            }
        }


        public void OnItemValueComboBoxSelectionChanged(int iIndex)
        {
            if (_SelectedConfigNode != null)
            {
                if (ItemValueComboBoxItems.Count > 0)
                {
                    string strValue = ItemValueComboBoxItems[iIndex].Value;
                    _SelectedConfigNode.Value = strValue;
                    _ItemValueText = strValue;
                }
            }
        }

    }
}
