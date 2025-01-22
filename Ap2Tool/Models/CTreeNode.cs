using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ap2Tool.Models
{
    public partial class CTreeNode : ObservableObject
    {
        public string Title { get; set; }
        public object? Value { get; set; }

        [ObservableProperty]
        public bool _IsExpanded;

        public ObservableCollection<CTreeNode>? SubNodes { get; set; }

        public CTreeNode(string title, 
            object? value = null,
            ObservableCollection<CTreeNode>? subNodes = null)
        {
            Title = title;
            Value = value;
            IsExpanded = false;

            if (subNodes != null)
                SubNodes = subNodes;
            else
                SubNodes = [];
        }

        public void SetExpanded(bool bValue, bool bWithChilds)
        {
            IsExpanded = bValue;

            if (SubNodes != null && bWithChilds)
            {
                foreach (var subNode in SubNodes)
                    subNode.SetExpanded(bValue, bWithChilds);
            }
        }

    }
}
