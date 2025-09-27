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
        
            /* 隐藏整个控制栏 */
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
        
            /* 代码雨容器 */
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
                0% {
                    transform: translateY(-100px);
                }
                100% {
                    transform: translateY(100vh);
                }
            }
        
            @keyframes charFade {
                0% {
                    opacity: 0;
                }
                10% {
                    opacity: 1;
                }
                90% {
                    opacity: 0.8;
                }
                100% {
                    opacity: 0;
                }
            }
        
            /* 处理中内容 */
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
        
            /* Loading 转圈圈 */
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
        
            /* 结算完成白屏 */
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
                z-index: 300;
            }
        
            .completion-loading {
                width: 50px;
                height: 50px;
                border: 4px solid rgba(0, 0, 0, 0.1);
                border-radius: 50%;
                border-top-color: #000;
                animation: spin 1s ease-in-out infinite;
            }

            /* 预加载指示器 */
            .preload-indicator {
                position: absolute;
                top: 10px;
                right: 10px;
                color: #cccccc; /* 浅灰色 */
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
                <div class=""preload-indicator"" id=""preloadIndicator"">预加载中...</div>
            
                <!-- 结算中屏幕 -->
                <div class=""settlement-screen"" id=""settlementScreen"">
                    <div class=""matrix-rain"" id=""matrixRain""></div>
                    <div class=""processing-content"">
                        <div class=""processing-text"">任务正在处理…</div>
                        <div class=""loading-spinner""></div>
                    </div>
                </div>
            
                <!-- 结算完成白屏 -->
                <div class=""completion-screen"" id=""completionScreen"">
                    <div class=""completion-loading""></div>
                </div>
            </div>
        </div>

        <script>
            let videoElement = document.getElementById('videoPlayer');
            let settlementScreen = document.getElementById('settlementScreen');
            let completionScreen = document.getElementById('completionScreen');
            let matrixRain = document.getElementById('matrixRain');
            let preloadIndicator = document.getElementById('preloadIndicator');
        
            let videoList = [];
            let currentVideoIndex = 0;
            let lastTime = 0;
        
            // 26个英文字母
            const chars = 'abcdefghijklmnopqrstuvwxyz';
        
            // 创建代码雨效果
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
            
                // 创建字符
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

            // 强制全量加载Blob视频
            function forcePreloadVideo(url) {
                return new Promise((resolve, reject) => {
                    const xhr = new XMLHttpRequest();
                    xhr.open('GET', url, true);
                    xhr.responseType = 'blob';
                
                    xhr.onprogress = function(event) {
                        if (event.lengthComputable) {
                            const percent = (event.loaded / event.total) * 100;
                            preloadIndicator.textContent = `预加载中... ${percent.toFixed(1)}%`;
                            preloadIndicator.style.display = 'block';
                        }
                    };
                
                    xhr.onload = function() {
                        if (xhr.status === 200) {
                            const blob = xhr.response;
                            const blobUrl = URL.createObjectURL(blob);
                            preloadIndicator.style.display = 'none';
                            resolve(blobUrl);
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

            // 检查并预加载视频
            async function preloadVideoCompletely(url) {
                try {
                    // 如果是blob URL，直接使用
                    if (url.startsWith('blob:')) {
                        return url;
                    }
                
                    // 强制全量加载
                    const blobUrl = await forcePreloadVideo(url);
                    return blobUrl;
                } catch (error) {
                    console.error('预加载失败:', error);
                    return url; // 失败时返回原URL
                }
            }
        
            // 初始化播放器
            function initializePlayer(videos) {
                videoList = videos;
                if (videoList.length > 0) {
                    playVideo(0);
                }

                // 设置强制预加载参数
                videoElement.preload = ""auto"";
                videoElement.load();
                videoElement.playbackRate = 1.0;
            
                // 禁用键盘快捷键
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
            
                // 禁用右键菜单和拖拽
                videoElement.addEventListener('contextmenu', function(e) {
                    e.preventDefault();
                    return false;
                });
            
                videoElement.addEventListener('dragstart', function(e) {
                    e.preventDefault();
                    return false;
                });
            
                videoElement.addEventListener('click', function(e) {
                    e.preventDefault();
                    e.stopPropagation();
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
        
            // 播放指定索引的视频（强制预加载版本）
            async function playVideo(index) {
                if (index >= videoList.length) {
                    console.log('所有视频播放完毕');
                    return;
                }
            
                currentVideoIndex = index;
                const originalUrl = videoList[index];
            
                try {
                    // 显示预加载指示器
                    preloadIndicator.style.display = 'block';
                    preloadIndicator.textContent = '预加载中...';
                
                    // 强制全量预加载
                    const videoUrl = await preloadVideoCompletely(originalUrl);
                
                    videoElement.src = videoUrl;
                    videoElement.playbackRate = 1.0;
                    lastTime = 0;
                
                    videoElement.onloadeddata = () => {
                        preloadIndicator.style.display = 'none';
                        videoElement.play().catch(e => {
                            console.log('自动播放被阻止，等待用户交互');
                        });
                    };
                
                    videoElement.oncanplaythrough = () => {
                        console.log('视频可以流畅播放，预加载完成');
                    };
                
                    videoElement.onended = () => {
                        showSettlementScreen();
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

                    // 监听缓冲事件
                    videoElement.onprogress = function() {
                        if (videoElement.buffered.length > 0) {
                            const bufferedEnd = videoElement.buffered.end(videoElement.buffered.length - 1);
                            const duration = videoElement.duration;
                            if (duration > 0) {
                                const bufferedPercent = (bufferedEnd / duration) * 100;
                                console.log(`已缓冲: ${bufferedPercent.toFixed(1)}%`);
                            }
                        }
                    };

                } catch (error) {
                    console.error('视频播放失败:', error);
                    preloadIndicator.style.display = 'none';
                }
            }
        
            // 显示结算屏幕
            function showSettlementScreen() {
                settlementScreen.style.display = 'flex';
                completionScreen.style.display = 'none';
            
                // 开始代码雨效果
                createMatrixRain();
            
                // 通知WPF开始结算任务
                window.chrome.webview.postMessage(JSON.stringify({
                    action: 'settlementComplete',
                    videoId: currentVideoIndex
                }));
            }
        
            // 处理结算结果
            function settlementResult(success) {
                if (success) {
                    // 显示结算完成白屏
                    settlementScreen.style.display = 'none';
                    completionScreen.style.display = 'flex';
                
                    // 2秒后播放下一个视频
                    setTimeout(() => {
                        completionScreen.style.display = 'none';
                        playVideo(currentVideoIndex + 1);
                    }, 2000);
                } else {
                    // 结算失败，5秒后重试
                    setTimeout(() => {
                        settlementScreen.style.display = 'none';
                        playVideo(currentVideoIndex);
                    }, 5000);
                }
            }
        
            // 清理Blob URL避免内存泄漏
            function cleanupBlobUrl() {
                if (videoElement.src && videoElement.src.startsWith('blob:')) {
                    URL.revokeObjectURL(videoElement.src);
                }
            }
        
            // 初始化
            window.addEventListener('DOMContentLoaded', () => {
                videoElement.removeAttribute('controls');
                videoElement.disablePictureInPicture = true;
            
                // 设置更积极的预加载策略
                videoElement.preload = ""auto"";
                videoElement.load();

                // 页面卸载时清理资源
                window.addEventListener('beforeunload', cleanupBlobUrl);
            
                if (window.chrome && window.chrome.webview) {
                    window.chrome.webview.addEventListener('message', event => {
                        try {
                            const data = JSON.parse(event.data);
                            if (data.action === 'start') {
                                videoElement.play();
                            } else if (data.action === 'stop') {
                                videoElement.pause();
                            } else if (data.action === 'preload') {
                                // 主动预加载命令
                                if (videoList.length > 0) {
                                    preloadVideoCompletely(videoList[0]);
                                }
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