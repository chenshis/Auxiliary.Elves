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
                    await Task.Delay(2000);
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
    
        .progress-container {
            width: 300px;
            height: 6px;
            background: #e0e0e0;
            border-radius: 3px;
            margin: 20px 0;
            overflow: hidden;
        }
    
        .progress-bar {
            height: 100%;
            background: #0078d4;
            border-radius: 3px;
            width: 0%;
            transition: width 0.3s ease;
        }
    
        .progress-text {
            font-size: 14px;
            color: #666;
            margin-top: 10px;
        }

        .completion-screen {
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
            z-index: 400;
        }
    
        .completion-loading {
            width: 50px;
            height: 50px;
            border: 4px solid rgba(0, 0, 0, 0.1);
            border-radius: 50%;
            border-top-color: #000;
            animation: spin 1s ease-in-out infinite;
        }

        .completion-text {
            font-size: 18px;
            color: #333;
            margin-top: 20px;
            font-family: 'Microsoft YaHei', sans-serif;
        }

        .preload-indicator {
            position: absolute;
            top: 10px;
            right: 10px;
            color: #cccccc;
            background: rgba(0, 0, 0, 0.2);
            opacity: 0.7;
            padding: 5px 10px;
            border-radius: 3px;
            font-size: 12px;
            display: none;
            z-index: 100;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""video-container"">
            <video id=""videoPlayer"" preload=""auto""></video>
            <div class=""preload-indicator"" id=""preloadIndicator"">加载中...</div>
        
            <div class=""loading-screen"" id=""loadingScreen"">
                <div class=""loading-content"">
                    <div class=""loading-spinner-white""></div>
                    <div class=""loading-title"">视频加载中...</div>
                    <div class=""progress-container"">
                        <div class=""progress-bar"" id=""progressBar""></div>
                    </div>
                    <div class=""progress-text"" id=""progressText"">0%</div>
                </div>
            </div>
        
            <div class=""settlement-screen"" id=""settlementScreen"">
                <div class=""matrix-rain"" id=""matrixRain""></div>
                <div class=""processing-content"">
                    <div class=""processing-text"">任务正在处理…</div>
                    <div class=""loading-spinner""></div>
                </div>
            </div>
        
            <div class=""completion-screen"" id=""completionScreen"">
                <div class=""completion-loading""></div>
                <div class=""completion-text"">任务完成</div>
            </div>
        </div>
    </div>

    <script>
        let videoElement = document.getElementById('videoPlayer');
        let settlementScreen = document.getElementById('settlementScreen');
        let completionScreen = document.getElementById('completionScreen');
        let loadingScreen = document.getElementById('loadingScreen');
        let matrixRain = document.getElementById('matrixRain');
        let preloadIndicator = document.getElementById('preloadIndicator');
        let progressBar = document.getElementById('progressBar');
        let progressText = document.getElementById('progressText');
    
        let currentVideoUrl = null;
        let lastTime = 0;
        let currentBlobUrl = null;
        let isVideoLoaded = false;
        let lastProgressPercent = 0; // 新增：记录上次进度百分比

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

        function updateProgress(percent) {
            // 修复：确保进度条只前进不后退
            if (percent > lastProgressPercent) {
                lastProgressPercent = percent;
                progressBar.style.width = percent + '%';
                progressText.textContent = percent.toFixed(1) + '%';
            }
        }

        function showLoadingScreen() {
            loadingScreen.style.display = 'flex';
            lastProgressPercent = 0; // 重置进度记录
            progressBar.style.width = '0%';
            progressText.textContent = '0%';
        }

        function hideLoadingScreen() {
            loadingScreen.style.display = 'none';
        }

        function loadSingleVideo(url) {
            return new Promise((resolve, reject) => {
                cleanupCurrentBlobUrl();
                isVideoLoaded = false;
                
                const xhr = new XMLHttpRequest();
                xhr.open('GET', url, true);
                xhr.responseType = 'blob';
            
                xhr.onprogress = function(event) {
                    if (event.lengthComputable) {
                        const percent = (event.loaded / event.total) * 100;
                        updateProgress(percent);
                    }
                };
            
                xhr.onload = function() {
                    if (xhr.status === 200) {
                        const blob = xhr.response;
                        currentBlobUrl = URL.createObjectURL(blob);
                        updateProgress(100); // 确保显示100%
                        resolve(currentBlobUrl);
                    } else {
                        reject(new Error('视频加载失败'));
                    }
                };
            
                xhr.onerror = function() {
                    reject(new Error('网络错误'));
                };
            
                xhr.send();
            });
        }

        function initializePlayer(videoUrl) {
            currentVideoUrl = videoUrl;
            
            videoElement.removeAttribute('controls');
            videoElement.disablePictureInPicture = true;
            videoElement.preload = ""auto"";
            videoElement.playbackRate = 1.0;
        
            document.addEventListener('keydown', function(e) {
                if (e.code === 'Space' || 
                    e.code === 'ArrowLeft' || e.code === 'ArrowRight' || 
                    e.code === 'ArrowUp' || e.code === 'ArrowDown' ||
                    e.code === 'PageUp' || e.code === 'PageDown' ||
                    e.code === 'Home' || e.code === 'End' ||
                    e.code === 'KeyK' || e.code === 'KeyM' ||
                    e.code === 'KeyF') {
                    e.preventDefault();
                    e.stopPropagation();
                }
            });
        
            videoElement.addEventListener('contextmenu', function(e) {
                e.preventDefault();
                return false;
            });
        
            videoElement.addEventListener('dragstart', function(e) {
                e.preventDefault();
                return false;
            });
        
            window.chrome.webview.addEventListener('message', event => {
                try {
                    const data = JSON.parse(event.data);
                    if (data.action === 'start') {
                        console.log('收到开始播放命令');
                        if (isVideoLoaded) {
                            videoElement.play().catch(e => {
                                console.log('播放被阻止:', e);
                            });
                        }
                    } else if (data.action === 'stop') {
                        console.log('收到停止播放命令');
                        videoElement.pause();
                        cleanupCurrentBlobUrl();
                    } else if (data.action === 'loadVideo') {
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
            
            if (currentVideoUrl) {
                loadAndPlayVideo(currentVideoUrl);
            }
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
            
                const finalVideoUrl = await loadSingleVideo(videoUrl);
                
                videoElement.src = finalVideoUrl;
                lastTime = 0;
            
                videoElement.onloadeddata = () => {
                    console.log('视频加载完成，准备播放');
                    isVideoLoaded = true;
                    
                    hideLoadingScreen();
                    
                    notifyWPF('videoReady', {
                        videoUrl: currentVideoUrl
                    });
                    
                    videoElement.play().catch(e => {
                        console.log('自动播放被阻止:', e);
                        notifyWPF('playbackBlocked', {
                            error: e.message
                        });
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
            
                videoElement.ontimeupdate = function() {
                    const currentTime = Math.floor(videoElement.currentTime);
                    if (currentTime > lastTime + 1) {
                        videoElement.currentTime = lastTime;
                    } else {
                        lastTime = currentTime;
                    }
                };
            
                videoElement.onseeking = function() {
                    videoElement.currentTime = lastTime;
                };
            
                videoElement.onratechange = function() {
                    if (videoElement.playbackRate !== 1.0) {
                        videoElement.playbackRate = 1.0;
                    }
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
            completionScreen.style.display = 'none';
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
                completionScreen.style.display = 'flex';
            
                setTimeout(() => {
                    completionScreen.style.display = 'none';
                    cleanupCurrentBlobUrl();
                    notifyWPF('VideoEnd', {
                        videoUrl: currentVideoUrl
                    });
                }, 1000);
            } else {
                setTimeout(() => {
                    settlementScreen.style.display = 'none';
                    console.log('结算失败，重新播放视频');
                    if (isVideoLoaded) {
                        videoElement.currentTime = 0;
                        videoElement.play();
                    } else {
                        loadAndPlayVideo(currentVideoUrl);
                    }
                }, 5000);
            }
        }
    
        function cleanupCurrentBlobUrl() {
            if (currentBlobUrl) {
                URL.revokeObjectURL(currentBlobUrl);
                currentBlobUrl = null;
                isVideoLoaded = false;
                console.log('已清理视频资源');
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
            window.addEventListener('beforeunload', cleanupCurrentBlobUrl);
        
            setTimeout(() => {
                notifyWPF('pageReady', {});
            }, 100);
        });
    </script>
</body>
</html>";
    }
}