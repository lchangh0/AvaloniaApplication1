using CommunityToolkit.Mvvm.ComponentModel;

namespace Ap2Tool.ViewModels
{
    public class ViewModelBase : ObservableObject
    {
        object _view;

        public void SetView(object view)
        {
            _view = view;
        }

        public T GetView<T>() where T : class
        {
            return (T)_view;
        }

    }
}
