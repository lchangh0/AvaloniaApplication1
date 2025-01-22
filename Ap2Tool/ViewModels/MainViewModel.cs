using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ap2Tool.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        
        public string Version
        {
            get { return "Ver. " + CApp.VERSION; }
        }


    }
}
