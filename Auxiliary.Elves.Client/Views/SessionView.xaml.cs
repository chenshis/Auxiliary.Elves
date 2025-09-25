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
            Closing += SessionView_Closing;
        }

        private void SessionView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = this.DataContext as SessionViewModel;
            if (viewModel != null)
            {
                viewModel.ExecutePublishMessage();
            }
        }

        public async Task InitializeWebView2Async()
        {
            var viewModel = this.DataContext as SessionViewModel;
            try
            {
                viewModel.RecordInfo($"账号：{viewModel.Account.AccountId}初始换窗口");

                this.Title = viewModel.Account.AccountId;
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
                var videoAddress = await viewModel.GetVideoAddressAsync();
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
                    var success = await viewModel.UpdatePoints();
                    // 通知WebView结算结果
                    string resultScript = $"settlementResult({success.ToString().ToLower()});";
                    await webView?.ExecuteScriptAsync(resultScript);
                    // 设置初始视频列表

                    var videoAddress = await viewModel.GetVideoAddressAsync();
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
        
        /* 隐藏播放速度控制 */
        video::-webkit-media-controls-playback-rate-button {
            display: none !important;
        }
        
        /* 隐藏全屏按钮 */
        video::-webkit-media-controls-fullscreen-button {
            display: none !important;
        }
        
        /* 禁用进度条拖拽手柄 */
        video::-webkit-media-controls-timeline::-webkit-slider-thumb {
            display: none !important;
            visibility: hidden !important;
            pointer-events: none !important;
        }
        
        /* 禁用进度条交互但保持显示 */
        video::-webkit-media-controls-timeline {
            pointer-events: none !important;
            cursor: default !important;
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
            <video id=""videoPlayer"" controls controlsList=""nodownload noremoteplayback noplaybackrate nofullscreen"" disablePictureInPicture></video>
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
        let lastTime = 0;
        
        // 初始化播放器
        function initializePlayer(videos) {
            videoList = videos;
            if (videoList.length > 0) {
                playVideo(0);
            }
            
            // 设置播放速度为正常速度
            videoElement.playbackRate = 1.0;
            
            // 禁用键盘快捷键
            videoElement.addEventListener('keydown', function(e) {
                // 禁用空格键播放/暂停
                if (e.code === 'Space') {
                    e.preventDefault();
                }
                // 禁用方向键快进快退
                if (e.code === 'ArrowLeft' || e.code === 'ArrowRight' || 
                    e.code === 'ArrowUp' || e.code === 'ArrowDown') {
                    e.preventDefault();
                }
                // 禁用PageUp/PageDown
                if (e.code === 'PageUp' || e.code === 'PageDown') {
                    e.preventDefault();
                }
                // 禁用Home/End键
                if (e.code === 'Home' || e.code === 'End') {
                    e.preventDefault();
                }
            });
            
            // 禁用右键菜单
            videoElement.addEventListener('contextmenu', function(e) {
                e.preventDefault();
                return false;
            });
            
            // 禁用拖拽事件
            videoElement.addEventListener('dragstart', function(e) {
                e.preventDefault();
                return false;
            });
            
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
            
            // 重置播放速度
            videoElement.playbackRate = 1.0;
            lastTime = 0;
            
            videoElement.onloadeddata = () => {
                videoElement.play();
            };
            
            videoElement.onended = () => {
                // 视频播放完成，显示结算屏幕
                showSettlementScreen();
            };
            
            // 监听时间更新事件，防止快进
            videoElement.ontimeupdate = function() {
                const currentTime = Math.floor(videoElement.currentTime);
                
                // 如果时间跳跃超过1秒，认为是快进操作，重置时间
                if (currentTime > lastTime + 1) {
                    videoElement.currentTime = lastTime;
                } else {
                    lastTime = currentTime;
                }
            };
            
            // 监听快进尝试（拖拽进度条）
            videoElement.onseeking = function() {
                // 立即重置到上一次有效的时间点
                videoElement.currentTime = lastTime;
            };
            
            // 监听播放速率变化
            videoElement.onratechange = function() {
                if (videoElement.playbackRate !== 1.0) {
                    videoElement.playbackRate = 1.0;
                }
            };
            
            // 禁用控制栏点击事件（特别是进度条区域）
            videoElement.addEventListener('click', function(e) {
                // 阻止进度条区域的点击事件
                const controls = videoElement.controls;
                if (controls) {
                    e.preventDefault();
                    e.stopPropagation();
                }
            }, true);
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
                    playVideo(currentVideoIndex + 1);
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
            // 设置视频元素属性以禁用控制
            videoElement.controlsList = 'nodownload noremoteplayback noplaybackrate nofullscreen';
            videoElement.disablePictureInPicture = true;
            
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