using Ap2Tool.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ap2Tool.ViewModels
{
    public partial class ConfigurationViewModel : ViewModelBase
    {
        public string Title { get; set; } = "Configuration";

        [ObservableProperty]
        List<CTreeNode> _Nodes;

        public ConfigurationViewModel()
        {
            _Nodes = new List<CTreeNode>();
            BuildTree();
        }

        void BuildTree()
        {
            _Nodes.Add(new CTreeNode("Section1")
            {
                SubNodes = new List<CTreeNode>()
                {
                    new CTreeNode("Item11")
                    { 
                        SubNodes = new List<CTreeNode>()
                        {
                            new CTreeNode("Item11a"),
                            new CTreeNode("Item11b"),
                            new CTreeNode("Item11c"),
                        }
                    },
                    new CTreeNode("Item12"),
                    new CTreeNode("Item13"),
                }
            });

            _Nodes.Add(new CTreeNode("Section2")
            {
                SubNodes = new List<CTreeNode>()
                {
                    new CTreeNode("Item21"),
                    new CTreeNode("Item32"),
                    new CTreeNode("Item23"),
                }
            });

            _Nodes.Add(new CTreeNode("Section3"));
        }


        [RelayCommand]
        public void Search(string strCondition)
        {
            int dummy = 0;
        }
    }
}
