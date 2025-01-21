using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Ap2Tool.ViewModels;
using System;

namespace Ap2Tool
{
    public class ViewLocator : IDataTemplate
    {
        // ViewModel에 대한 View를 생성한다.
        public Control? Build(object? param)
        {
            if (param is null)
                return null;

            ViewModelBase? vmBase = param as ViewModelBase;
            if (vmBase is null)
                return null;

            var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            if (type != null)
            {
                // View 인스턴스를 만든다. (파라미터로 ViewModel을 전달한다.)
                Control? view = (Control?)Activator.CreateInstance(type, param);

                if (view != null)
                    vmBase.SetView(view);

                return view;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
