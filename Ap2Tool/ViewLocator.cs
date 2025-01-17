using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Ap2Tool.ViewModels;
using System;

namespace Ap2Tool
{
    public class ViewLocator : IDataTemplate
    {

        public Control? Build(object? param)
        {
            if (param is null)
                return null;

            var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            if (type != null)
            {
                // View 인스턴스를 만든다. (파라미터로 ViewModel을 전달한다.)
                return (Control)Activator.CreateInstance(type, param)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
