using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Ap2Tool.ViewModels;
using System;

namespace Ap2Tool
{
    public class ViewLocator : IDataTemplate
    {
        // ViewModel�� ���� View�� �����Ѵ�.
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
                // View �ν��Ͻ��� �����. (�Ķ���ͷ� ViewModel�� �����Ѵ�.)
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
