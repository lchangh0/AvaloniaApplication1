using Ap2Tool.Models;
using ApCommon;
using Avalonia.Controls;
using Avalonia.Input.TextInput;
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


        [ObservableProperty]
        string _ItemTitle;

        [ObservableProperty]
        string _ItemValueText;

        [ObservableProperty]
        bool _ItemValueBool;

        [ObservableProperty]
        string _ItemValueBoolTitle;

        public class CComboBoxItem
        {
            public string Value { get; set; }
            public string Display { get; set; }
        }

        [ObservableProperty]
        // 주의) ObservableCollection<> 형식을 사용해야 한다.
        ObservableCollection<CComboBoxItem> _ItemValueComboBoxItems;

        CComboBoxItem _SelectedItem;
        public CComboBoxItem SelectedItem
        {
            get { return _SelectedItem; }
            set 
            {
                bool bChanged = _SelectedItem != value;
                SetProperty(ref _SelectedItem, value);
                if (bChanged)
                    OnSelectedItemChanged(value);
            }
        }

        void OnSelectedItemChanged(CComboBoxItem item)
        {
            if (item == null)
                return;

            string strValue = item.Value;
            _SelectedConfigNode.Value = strValue;
            _ItemValueText = strValue;
        }


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


        [ObservableProperty]
        string _SearchButtonText = "Search";

        [ObservableProperty]
        string _SearchNavPrevButtonText;

        [ObservableProperty]
        string _SearchNavNextButtonText;

        [ObservableProperty]
        bool _SearchNavPrevButtonEnabled;

        [ObservableProperty]
        bool _SearchNavNextButtonEnabled;

        [ObservableProperty]
        string _SearchResText;


        public ConfigurationViewModel()
        {
            _TreeRoot = new CTreeNode("");
            ItemDescriptionTitle = "";
            _ItemValueComboBoxItems = new ObservableCollection<CComboBoxItem>();

            ApplyResourceText();
        }

                
        CConfig m_config;
        CConfig m_configLoaded;

        public bool Initialize()
        {
            m_config = new CConfig(Program.Config);
            m_configLoaded = new CConfig(m_config);

            ClearItemDisplay();

            if (!InitTreeviewAndShow())
                return false;

            return true;
        }

        void ApplyResourceText()
        {
            _SearchButtonText = CResource.GetString(CResource.IDS_SEARCH);
            _SearchNavPrevButtonText = "<";
            _SearchNavNextButtonText = ">";
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
                string strText = configNode.GetTitle(CResource.Lang == "ko");
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


        CTreeNode _TreeViewSelectedItem;
        public CTreeNode TreeViewSelectedItem
        {
            get { return _TreeViewSelectedItem; }
            set { SetProperty(ref _TreeViewSelectedItem, value); }
        }

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

                    ItemTitle = configNode.GetTitle(CResource.Lang == "ko");

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

                            foreach (var item in configNode.EnumValues)
                            {
                                ItemValueComboBoxItems.Add(new CComboBoxItem() { Value = item, Display = item });
                            }

                            SelectedItem = null;
                            foreach (var item in ItemValueComboBoxItems)
                            {
                                if (item.Value == configNode.Value)
                                {
                                    SelectedItem = item;
                                    break;
                                }
                            }

                            ItemValueEnumVisible = true;
                        }
                        else
                            ItemValueTextVisible = true;
                    }

                    ItemDescription = configNode.GetDescription(CResource.Lang == "ko");
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


        List<CTreeNode> m_SearchResult = new List<CTreeNode>();
        int m_iSearchIdx = 0;
        TreeView m_treeView;
        
        public async void OnSearchButtonClick(TreeView treeView, string strCondition)
        {
            if (treeView == null)
                return;

            //SearchOptionName searchOption = GetSearchOption();

            m_treeView = treeView;
            m_SearchResult.Clear();

            List<CTreeNode> items = new List<CTreeNode>();
            foreach (var tvItem in treeView.Items)
                items.Add(tvItem as CTreeNode);

            SearchNode(items, strCondition, m_SearchResult);

            if (m_SearchResult.Count > 0)
            {
                SearchNavPrevButtonEnabled = true;
                SearchNavNextButtonEnabled= true;
                m_iSearchIdx = 0;
                UpdateSearchResText();

                SelectTreeNode(treeView, m_SearchResult[m_iSearchIdx]);
            }
            else
            {
                SearchNavPrevButtonEnabled = false;
                SearchNavNextButtonEnabled = false;
                SearchResText = "";
            }
        }

        void UpdateSearchResText()
        {
            if (m_SearchResult != null && m_SearchResult.Count > 0)
            {
                SearchResText = string.Format("{0}/{1}",
                    m_iSearchIdx + 1,
                    m_SearchResult.Count);
            }
            else
                SearchResText = "";
        }

        void SearchNode(List<CTreeNode> items, string strCondition,
            List<CTreeNode> foundItems)
        {
            if (items == null || items.Count == 0)
                return;

            if (string.IsNullOrEmpty(strCondition))
                return;

            foreach (CTreeNode item in items)
            {
                bool bFound = false;
                CConfigNode? configNode = item.Value as CConfigNode;
                if (configNode == null)
                    continue;

                if (configNode.Title.Contains(strCondition, StringComparison.OrdinalIgnoreCase) ||
                    configNode.TitleKor.Contains(strCondition, StringComparison.OrdinalIgnoreCase))
                    bFound = true;

                if (!string.IsNullOrEmpty(configNode.GetTitleRes()) &&
                    configNode.GetTitleRes().Contains(strCondition, StringComparison.OrdinalIgnoreCase))
                    bFound = true;

                if (configNode.Description.Contains(strCondition, StringComparison.OrdinalIgnoreCase) ||
                    configNode.DescriptionKor.Contains(strCondition, StringComparison.OrdinalIgnoreCase))
                    bFound = true;

                if (!string.IsNullOrEmpty(configNode.GetDescriptionRes()) &&
                    configNode.GetDescriptionRes().Contains(strCondition, StringComparison.OrdinalIgnoreCase))
                    bFound = true;

                if (bFound)
                {
                    foundItems.Add(item);
                }

                SearchNode(item.SubNodes, strCondition, foundItems);
            }
        }


        void SelectTreeNode(TreeView treeView, CTreeNode treeNode)
        {
            //treeView.SelectedItem = treeNode;
            TreeViewSelectedItem = treeNode;
        }

        [RelayCommand]
        public void OnClickSearchNavPrev()
        {
            if (m_iSearchIdx > 0)
            {
                m_iSearchIdx--;
                UpdateSearchResText();
                SelectTreeNode(m_treeView, m_SearchResult[m_iSearchIdx]);
            }
        }

        [RelayCommand]
        public void OnClickSearchNavNext()
        {
            if (m_iSearchIdx < m_SearchResult.Count - 1)
            {
                m_iSearchIdx++;
                UpdateSearchResText();
                SelectTreeNode(m_treeView, m_SearchResult[m_iSearchIdx]);
            }
        }


    }
}
