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
        void ShowWindow<TViewModel, TParameter>(TParameter parameter) where TViewModel : class;
    }

    public class WindowService : IWindowService
    {
        private readonly IContainerProvider _container;

        public WindowService(IContainerProvider container)
        {
            _container = container;
        }

        public void ShowWindow<TViewModel, TParameter>(TParameter parameter)
            where TViewModel : class
        {
            var window = _container.Resolve<Window>(typeof(TViewModel).Name);
            if (window.DataContext is IParameterReceiver receiver)
            {
                receiver.ApplyParameters(parameter);
            }
            window.Show();
        }
    }

    public interface IParameterReceiver
    {
        void ApplyParameters<TParameter>(TParameter parameter);
    }
}
