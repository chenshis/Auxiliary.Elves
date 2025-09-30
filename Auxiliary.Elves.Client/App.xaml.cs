using Auxiliary.Elves.Client.ViewModels;
using Auxiliary.Elves.Client.Views;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
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
        private const string AppMutexName = "YourCompany.YourApp.SingleInstanceMutex";
        private static Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            // 尝试创建互斥体
            bool createdNew;
            _mutex = new Mutex(true, AppMutexName, out createdNew);

            if (!createdNew)
            {
                // 互斥体已存在，说明程序已在运行
                MessageBox.Show("应用程序已经在运行中！", "提示",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                // 退出当前实例
                Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }

        protected override Window CreateShell()
        {
            // 通过容器创造主界面实例
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IWindowService, WindowService>();
            containerRegistry.Register<Window, SessionView>(nameof(SessionViewModel));
            containerRegistry.RegisterDialog<AddUserDialogView>();
            // service collection 集合转换
            if (containerRegistry is IContainerExtension container)
            {
                // 注入 httpClient
                container.CreateServiceProvider((services) =>
                {
                    services.AddHttpClient();
                    services.AddLogging(configure =>
                    {
                        configure.ClearProviders();
                        configure.SetMinimumLevel(LogLevel.Trace);
                        configure.AddNLog();
                    });
                });
            }
            // httpclient帮助类注册
            containerRegistry.RegisterSingleton<AuxElvesHttpClient>();
        }
    }

}
