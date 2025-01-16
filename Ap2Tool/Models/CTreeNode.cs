﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ap2Tool.Models
{
    public class CTreeNode
    {
        public string Title { get; set; }
        public object? Value { get; set; }
        public List<CTreeNode>? SubNodes { get; set; }

        public CTreeNode(string title, 
            object? value = null, 
            List<CTreeNode>? subNodes = null)
        {
            Title = title;
            Value = value;
            SubNodes = subNodes;
        }
    }
}
