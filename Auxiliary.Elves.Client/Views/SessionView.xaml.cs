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
using Newtonsoft.Json;

namespace Auxiliary.Elves.Client.Views
{
    public partial class SessionView : HandyControl.Controls.Window
    {
        private readonly Random _random = new Random();
        private bool _isWebViewReady = false;

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
                webView.WebMessageReceived += WebView_WebMessageReceived;

                webView.NavigateToString(HtmlContent);
                _isWebViewReady = true;
                viewModel.RecordInfo("WebView2初始化完成，等待页面就绪");

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



        private async void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var webView = sender as WebView2;
            string message = e.TryGetWebMessageAsString();

            try
            {
                var viewModel = this.DataContext as SessionViewModel;
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(message);
                string action = data.action.ToString();
                if (action == "pageReady")
                {
                    await SendNextVideo(); // 发送第一个视频
                }
                else if (action == "settlementComplete")
                {

                    // 执行结算任务
                    var success = await viewModel.UpdatePoints();
                    var nextSec = _random.Next(10, 20);
                    viewModel.RecordInfo($"{viewModel.Account.AccountId} :等待时间:{nextSec}");
                    await Task.Delay(TimeSpan.FromSeconds(nextSec));
                    // 通知WebView结算结果
                    string resultScript = $"settlementResult({success.ToString().ToLower()});";
                    await webView?.ExecuteScriptAsync(resultScript);
                    // 设置初始视频列表
                    await SendNextVideo(); // 发送第一个视频
                }
                else if (action == "videoError")
                {
                    viewModel.RecordInfo($"视频播放错误: {data.error}");
                    // 出错时重试下一个视频
                    await Task.Delay(1000);
                    await SendNextVideo();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async Task SendNextVideo()
        {
            if (!_isWebViewReady) return;

            var viewModel = this.DataContext as SessionViewModel;
            try
            {
                var videoAddress = await viewModel.GetVideoAddressAsync();
                if (!string.IsNullOrEmpty(videoAddress))
                {
                    SendLoadVideoCommand(videoAddress);
                    viewModel.RecordInfo($"发送视频地址: {videoAddress}");
                }
                else
                {
                    viewModel.RecordInfo("获取视频地址为空");
                }
            }
            catch (Exception ex)
            {
                viewModel.RecordError(ex, "获取视频地址失败");
            }
        }

        private void SendLoadVideoCommand(string videoUrl)
        {
            if (!_isWebViewReady) return;

            try
            {
                var message = new { action = "loadVideo", videoUrl = videoUrl };
                SendMessageToWebView(message);
            }
            catch (Exception ex)
            {
                var viewModel = this.DataContext as SessionViewModel;
                viewModel?.RecordError(ex, "发送loadVideo命令失败");
            }
        }

        private void SendSettlementResult(bool success)
        {
            if (!_isWebViewReady) return;

            try
            {
                var message = new { action = "settlementResult", success = success };
                SendMessageToWebView(message);
            }
            catch (Exception ex)
            {
                var viewModel = this.DataContext as SessionViewModel;
                viewModel?.RecordError(ex, "发送结算结果失败");
            }
        }

        private void SendMessageToWebView(object message)
        {
            try
            {
                string json = JsonConvert.SerializeObject(message);
                webView.CoreWebView2.PostWebMessageAsString(json);
            }
            catch (Exception ex)
            {
                var viewModel = this.DataContext as SessionViewModel;
                viewModel?.RecordError(ex, "发送消息到WebView失败");
            }
        }

        public void Start()
        {
            if (_isWebViewReady)
            {
                SendMessageToWebView(new { action = "start" });
            }
        }

        public void Stop()
        {
            if (_isWebViewReady)
            {
                SendMessageToWebView(new { action = "stop" });
            }
        }
        public string HtmlContent { get; set; } =
 @"<!DOCTYPE html>
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
    
        video::-webkit-media-controls {
            display: none !important;
        }
    
        .settlement-screen {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: #000;
            display: none;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            z-index: 200;
            overflow: hidden;
        }
    
        .matrix-rain {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            pointer-events: none;
        }
    
        .code-column {
            position: absolute;
            top: -100px;
            font-family: 'Courier New', monospace;
            font-size: 16px;
            color: #0f0;
            animation: codeFall linear infinite;
        }
    
        .code-char {
            opacity: 0;
            animation: charFade linear infinite;
        }
    
        @keyframes codeFall {
            0% { transform: translateY(-100px); }
            100% { transform: translateY(100vh); }
        }
    
        @keyframes charFade {
            0% { opacity: 0; }
            10% { opacity: 1; }
            90% { opacity: 0.8; }
            100% { opacity: 0; }
        }
    
        .processing-content {
            position: relative;
            z-index: 10;
            text-align: center;
        }
    
        .processing-text {
            font-size: 24px;
            color: #0f0;
            margin-bottom: 30px;
            font-family: 'Microsoft YaHei', sans-serif;
        }
    
        .loading-spinner {
            width: 40px;
            height: 40px;
            border: 3px solid rgba(0, 255, 0, 0.3);
            border-radius: 50%;
            border-top-color: #0f0;
            animation: spin 1s ease-in-out infinite;
            margin: 0 auto;
        }
    
        @keyframes spin {
            to { transform: rotate(360deg); }
        }
    
        .loading-screen {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: #fff;
            display: none;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            z-index: 300;
        }
    
        .loading-content {
            text-align: center;
            color: #333;
        }
    
        .loading-title {
            font-size: 24px;
            margin-bottom: 30px;
            font-family: 'Microsoft YaHei', sans-serif;
        }
    
        .loading-spinner-white {
            width: 50px;
            height: 50px;
            border: 4px solid rgba(0, 0, 0, 0.1);
            border-radius: 50%;
            border-top-color: #000;
            animation: spin 1s ease-in-out infinite;
            margin: 0 auto 20px;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""video-container"">
            <video id=""videoPlayer"" preload=""auto"" autoplay muted></video>
        
            <div class=""loading-screen"" id=""loadingScreen"">
                <div class=""loading-content"">
                    <div class=""loading-spinner-white""></div>
                    <div class=""loading-title"">正在获取任务...</div>
                </div>
            </div>
        
            <div class=""settlement-screen"" id=""settlementScreen"">
                <div class=""matrix-rain"" id=""matrixRain""></div>
                <div class=""processing-content"">
                    <div class=""processing-text"">任务正在处理…</div>
                    <div class=""loading-spinner""></div>
                </div>
            </div>
        </div>
    </div>

    <script>
        let videoElement = document.getElementById('videoPlayer');
        let settlementScreen = document.getElementById('settlementScreen');
        let loadingScreen = document.getElementById('loadingScreen');
        let matrixRain = document.getElementById('matrixRain');
    
        let currentVideoUrl = null;
        let isVideoLoaded = false;

        const chars = 'abcdefghijklmnopqrstuvwxyz';
    
        function createMatrixRain() {
            matrixRain.innerHTML = '';
        
            const columns = Math.floor(window.innerWidth / 25);
        
            for (let i = 0; i < columns; i++) {
                createCodeColumn(i * 25);
            }
        }
    
        function createCodeColumn(left) {
            const column = document.createElement('div');
            column.className = 'code-column';
            column.style.left = left + 'px';
        
            const duration = 2 + Math.random() * 3;
            const delay = Math.random() * 2;
        
            column.style.animationDuration = duration + 's';
            column.style.animationDelay = delay + 's';
        
            const charCount = 20 + Math.floor(Math.random() * 15);
            for (let j = 0; j < charCount; j++) {
                const char = document.createElement('div');
                char.className = 'code-char';
                char.textContent = chars[Math.floor(Math.random() * chars.length)];
            
                const charDuration = 0.3 + Math.random() * 0.7;
                const charDelay = j * 0.08;
            
                char.style.animationDuration = charDuration + 's';
                char.style.animationDelay = charDelay + 's';
            
                column.appendChild(char);
            }
        
            matrixRain.appendChild(column);
        
            setTimeout(() => {
                if (column.parentNode) {
                    column.parentNode.removeChild(column);
                }
                createCodeColumn(left);
            }, (duration + delay) * 1000);
        }

        function showLoadingScreen() {
            loadingScreen.style.display = 'flex';
        }

        function hideLoadingScreen() {
            loadingScreen.style.display = 'none';
        }

        function initializePlayer() {
            videoElement.removeAttribute('controls');
            videoElement.disablePictureInPicture = true;
            videoElement.preload = ""auto"";
            videoElement.playbackRate = 1.0;
        
            // 监听消息
            window.chrome.webview.addEventListener('message', event => {
                try {
                    const data = JSON.parse(event.data);
                    if (data.action === 'loadVideo') {
                        console.log('收到加载视频命令:', data.videoUrl);
                        if (data.videoUrl) {
                            loadAndPlayVideo(data.videoUrl);
                        }
                    } else if (data.action === 'settlementResult') {
                        console.log('收到结算结果:', data.success);
                        settlementResult(data.success);
                    }
                } catch (e) {
                    console.error('Error parsing message:', e);
                }
            });
        }
    
        async function loadAndPlayVideo(videoUrl) {
            if (!videoUrl) {
                console.error('视频URL为空');
                return;
            }
        
            currentVideoUrl = videoUrl;
            isVideoLoaded = false;
        
            try {
                showLoadingScreen();
                console.log('开始加载视频:', videoUrl);
                
                // 添加1秒延迟，避免加载太快
                console.log('等待1秒避免加载太快...');
                await new Promise(resolve => setTimeout(resolve, 1000));
                console.log('延迟结束，开始设置视频源');
            
                // 直接设置视频源，不通过blob转换
                videoElement.src = videoUrl;
            
                videoElement.onloadeddata = () => {
                    console.log('视频加载完成，开始播放');
                    isVideoLoaded = true;
                    
                    hideLoadingScreen();
                    
                    notifyWPF('videoReady', {
                        videoUrl: currentVideoUrl
                    });
                    
                    // 强制播放
                    videoElement.play().catch(e => {
                        console.log('自动播放被阻止:', e);
                        // 如果自动播放失败，显示控件让用户点击
                        videoElement.controls = true;
                    });
                };
            
                videoElement.onended = () => {
                    console.log('视频播放完成');
                    showSettlementScreen();
                };
            
                videoElement.onerror = (e) => {
                    console.error('视频播放错误:', e);
                    hideLoadingScreen();
                    isVideoLoaded = false;
                    notifyWPF('videoError', {
                        error: '视频加载失败',
                        videoUrl: currentVideoUrl
                    });
                };

            } catch (error) {
                console.error('视频加载失败:', error);
                hideLoadingScreen();
                isVideoLoaded = false;
                notifyWPF('videoError', {
                    error: error.message,
                    videoUrl: currentVideoUrl
                });
            }
        }
    
        function showSettlementScreen() {
            console.log('显示结算屏幕');
            settlementScreen.style.display = 'flex';
            loadingScreen.style.display = 'none';
        
            createMatrixRain();
        
            notifyWPF('settlementComplete', {
                videoUrl: currentVideoUrl
            });
        }
    
        function settlementResult(success) {
            console.log('处理结算结果:', success);
            
            if (success) {
                settlementScreen.style.display = 'none';
                notifyWPF('VideoEnd', {
                    videoUrl: currentVideoUrl
                });
            } else {
                setTimeout(() => {
                    settlementScreen.style.display = 'none';
                    console.log('结算失败，重新播放视频');
                    if (currentVideoUrl) {
                        loadAndPlayVideo(currentVideoUrl);
                    }
                }, 5000);
            }
        }
    
        function notifyWPF(action, data) {
            if (window.chrome && window.chrome.webview) {
                const message = {
                    action: action,
                    timestamp: new Date().toISOString(),
                    ...data
                };
                console.log('发送消息到WPF:', message);
                window.chrome.webview.postMessage(JSON.stringify(message));
            } else {
                console.warn('WebView2不可用，无法发送消息');
            }
        }
    
        window.addEventListener('DOMContentLoaded', () => {
            initializePlayer();
            setTimeout(() => {
                notifyWPF('pageReady', {});
            }, 100);
        });
    </script>
</body>
</html>";
    }
}