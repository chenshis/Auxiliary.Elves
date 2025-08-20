using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Auxiliary.Elves.Client
{
    public interface IWindowService
    {
        void ShowWindow<TViewModel>() where TViewModel : class;
    }

    public class WindowService : IWindowService
    {
        private readonly IContainerProvider _container;

        public WindowService(IContainerProvider container)
        {
            _container = container;
        }

        public void ShowWindow<TViewModel>() where TViewModel : class
        {
            var window = _container.Resolve<Window>(typeof(TViewModel).Name);
            window.DataContext = _container.Resolve<TViewModel>();
            window.Show();
        }
    }
}
