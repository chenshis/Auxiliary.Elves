using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Auxiliary.Elves.Client.Models;
using Auxiliary.Elves.Client.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Auxiliary.Elves.Client.Views
{
    public partial class SessionView : HandyControl.Controls.Window
    {
        public SessionView()
        {
            InitializeComponent();
            Loaded += async (sender, e) => await InitializeWebView2Async();
        }

        public async Task InitializeWebView2Async()
        {
            var viewModel = this.DataContext as SessionViewModel;
            try
            {
                viewModel.RecordInfo($"绑定账号：{viewModel.Account.BindAccount}初始换窗口");

                this.Title = viewModel.Account.BindAccount;
                string userDataFolder = System.IO.Path.Combine(Environment.CurrentDirectory, "UserData", Guid.NewGuid().ToString());
                if (!Directory.Exists(userDataFolder))
                {
                    Directory.CreateDirectory(userDataFolder);
                }
                var env = await CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);
                await webView.EnsureCoreWebView2Async(env);
                // 初始化WebView2
                webView.NavigationCompleted += async (sender, e) => await WebView_NavigationCompleted(sender, e);
                webView.WebMessageReceived += WebView_WebMessageReceived;
                webView.Source = new Uri(System.IO.Path.GetFullPath("video_player.html"));
                await webView.EnsureCoreWebView2Async();
                webView.NavigateToString(HtmlContent);
                webView.CoreWebView2.PostWebMessageAsString("{\"action\":\"start\"}");

            }
            catch (Exception ex)
            {
                viewModel.RecordError(ex, "WebView2 initialization failed");

                // 提供用户友好的错误信息
                if (ex.Message.Contains("runtime") || ex.Message.Contains("not installed"))
                {
                    // 提示用户安装 WebView2 Runtime
                    MessageBox.Show("需要安装 WebView2 Runtime 才能正常运行。请从微软官网下载安装。",
                                   "运行时缺失", MessageBoxButton.OK, MessageBoxImage.Error);

                    // 可选：打开下载页面
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/",
                        UseShellExecute = true
                    });
                }
            }
        }

        private async Task WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            var webView = sender as WebView2;
            if (webView != null && webView.CoreWebView2 != null)
            {
                var viewModel = this.DataContext as SessionViewModel;
                var videoAddress = viewModel.GetVideoAddress();
                string[] videos = { videoAddress };
                string script = $"initializePlayer({Newtonsoft.Json.JsonConvert.SerializeObject(videos)});";
                await webView.ExecuteScriptAsync(script);
            }
        }

        private async void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var webView = sender as WebView2;
            string message = e.TryGetWebMessageAsString();

            try
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(message);
                string action = data.action.ToString();

                if (action == "settlementComplete")
                {
                    var viewModel = this.DataContext as SessionViewModel;
                    // 执行结算任务
                    await Task.Delay(2000); // 模拟耗时操作
                    var success = await viewModel.UpdatePoints();
                    // 通知WebView结算结果
                    string resultScript = $"settlementResult({success.ToString().ToLower()});";
                    await webView?.ExecuteScriptAsync(resultScript);
                    // 设置初始视频列表

                    var videoAddress = viewModel.GetVideoAddress();
                    string[] videos = { videoAddress };
                    string script = $"initializePlayer({Newtonsoft.Json.JsonConvert.SerializeObject(videos)});";
                    await webView.ExecuteScriptAsync(script);
                }
            }
            catch (Exception ex)
            {
            }
        }


        public void Start()
        {
            webView.CoreWebView2.PostWebMessageAsString("{\"action\":\"start\"}");
        }


        public void Stop()
        {
            webView.CoreWebView2.PostWebMessageAsString("{\"action\":\"stop\"}");
        }


        public string HtmlContent { get; set; }

       = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>视频播放器</title>
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: 'Segoe UI', Arial, sans-serif;
            background: #1e1e1e;
            color: white;
            overflow: hidden;
            position: relative;
        }
        
        .container {
            width: 100%;
            height: 100vh;
            display: flex;
            flex-direction: column;
        }
        .video-container {
            flex: 1;
            position: relative;
            background: #000;
        }
        
        video {
            width: 100%;
            height: 100%;
            object-fit: contain;
        }
        
        .settlement-screen {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(30, 30, 30, 0.95);
            display: none;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            z-index: 200;
        }
        
        .loading-spinner {
            width: 60px;
            height: 60px;
            border: 5px solid rgba(255, 255, 255, 0.3);
            border-radius: 50%;
            border-top-color: #4ca1af;
            animation: spin 1s ease-in-out infinite;
            margin-bottom: 20px;
        }
        
        @keyframes spin {
            to { transform: rotate(360deg); }
        }
        
        .settlement-text {
            font-size: 24px;
            font-weight: bold;
            color: #fff;
            text-align: center;
        }
        
        .success-message {
            color: #4CAF50;
            font-size: 20px;
            margin-top: 15px;
            display: none;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""video-container"">
            <video id=""videoPlayer"" controls></video>
            <div class=""settlement-screen"" id=""settlementScreen"">
                <div class=""loading-spinner""></div>
                <div class=""settlement-text"">结算任务执行中，请稍候...</div>
                <div class=""success-message"" id=""successMessage"">✓ 结算成功！</div>
            </div>
        </div>
    </div>

    <script>
        let videoElement = document.getElementById('videoPlayer');
        let settlementScreen = document.getElementById('settlementScreen');
        let successMessage = document.getElementById('successMessage');
        
        let videoList = [];
        let currentVideoIndex = 0;
        
        // 初始化播放器
        function initializePlayer(videos) {
            videoList = videos;
            if (videoList.length > 0) {
                playVideo(0);
            }
            
            // 监听来自WPF的消息
            window.chrome.webview.addEventListener('message', event => {
                try {
                    const data = JSON.parse(event.data);
                    if (data.action === 'start') {
                        videoElement.play();
                    } else if (data.action === 'stop') {
                        videoElement.pause();
                    }
                } catch (e) {
                    console.error('Error parsing message:', e);
                }
            });
        }
        
        // 播放指定索引的视频
        function playVideo(index) {
            if (index >= videoList.length) {
                console.log('所有视频播放完毕');
                return;
            }
            
            currentVideoIndex = index;
            videoElement.src = videoList[index];
            
            videoElement.onloadeddata = () => {
                videoElement.play();
            };
            
            videoElement.onended = () => {
                // 视频播放完成，显示结算屏幕
                showSettlementScreen();
            };
        }
        
        // 显示结算屏幕
        function showSettlementScreen() {
            settlementScreen.style.display = 'flex';
            successMessage.style.display = 'none';
            
            // 通知WPF开始结算任务
            window.chrome.webview.postMessage(JSON.stringify({
                action: 'settlementComplete',
                videoId: currentVideoIndex
            }));
        }
        
        // 处理结算结果
        function settlementResult(success) {
            if (success) {
                // 显示成功消息
                document.querySelector('.settlement-text').textContent = '结算完成！';
                document.querySelector('.loading-spinner').style.display = 'none';
                successMessage.style.display = 'block';
                
                // 2秒后播放下一个视频
                setTimeout(() => {
                    settlementScreen.style.display = 'none';
                    playVideo(currentVideoIndex);
                }, 2000);
            } else {
                // 处理结算失败
                document.querySelector('.settlement-text').textContent = '结算失败，请重试...';
                document.querySelector('.loading-spinner').style.display = 'none';
                
                // 5秒后重试
                setTimeout(() => {
                    settlementScreen.style.display = 'none';
                    playVideo(currentVideoIndex);
                }, 5000);
            }
        }
        
        // 初始化
        window.addEventListener('DOMContentLoaded', () => {
            // 等待WebView2注入对象
            if (window.chrome && window.chrome.webview) {
                window.chrome.webview.addEventListener('message', event => {
                    try {
                        const data = JSON.parse(event.data);
                        if (data.action === 'start') {
                            videoElement.play();
                        } else if (data.action === 'stop') {
                            videoElement.pause();
                        }
                    } catch (e) {
                        console.error('Error parsing message:', e);
                    }
                });
            }
        });
    </script>
</body>
</html>";


    }
}