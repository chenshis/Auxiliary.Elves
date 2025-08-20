using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Auxiliary.Elves.Client.Views
{
    /// <summary>
    /// SessionView.xaml 的交互逻辑
    /// </summary>
    public partial class SessionView : Window
    {
        public SessionView()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            // 为每个窗口创建独立的 userDataFolder
            string userDataFolder = Path.Combine(Environment.CurrentDirectory, "UserData", Guid.NewGuid().ToString());
            var env = await CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);
            await WebView.EnsureCoreWebView2Async(env);

            // 导航到示例网页
            WebView.CoreWebView2.Navigate("https://www.bing.com");
        }
    }
}

