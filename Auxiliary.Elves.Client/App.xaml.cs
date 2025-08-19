using Auxiliary.Elves.Client.ViewModels;
using Auxiliary.Elves.Client.Views;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.Windows;

namespace Auxiliary.Elves.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            // 通过容器创造主界面实例
            return Container.Resolve<MainView>();
        }

        // 初始化Shell（主窗口）的时候执行这个方法
        protected override void InitializeShell(Window shell)
        {

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new NotImplementedException();
        }
    }

}
