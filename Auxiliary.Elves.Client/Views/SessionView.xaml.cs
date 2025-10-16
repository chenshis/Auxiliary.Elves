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
                viewModel.RecordInfo($"{viewModel.Account.AccountId}初始化完成，等待页面就绪");

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
                else if (action == "videoTimeout")
                {
                    viewModel.RecordInfo($"{viewModel.Account.AccountId}视频播放错误或者超时: {data.error};视频地址：{data.videoUrl}");
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
                SendLoadVideoCommand(videoAddress);
            }
            catch (Exception ex)
            {
                SendLoadVideoCommand(null);
                viewModel.RecordError(ex, $"{viewModel.Account.AccountId}获取视频地址失败");
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
                viewModel?.RecordError(ex, $"{viewModel?.Account?.AccountId}发送结算结果失败");
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



        public string HtmlContent { get; set; } = @"<!DOCTYPE html>
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
    
        .matrix-canvas {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            pointer-events: none;
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
            display: flex;
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

        .countdown-text {
            font-size: 24px;
            color: #333;
            margin-top: 10px;
            font-family: 'Microsoft YaHei', sans-serif;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""video-container"">
            <video id=""videoPlayer"" preload=""auto"" muted style=""display: none;""></video>
        
            <div class=""loading-screen"" id=""loadingScreen"">
                <div class=""loading-content"">
                    <div class=""loading-spinner-white""></div>
                    <div class=""loading-title"">正在获取任务…</div>
                    <div class=""countdown-text"" id=""countdownText"" style=""display: none;""></div>
                </div>
            </div>
        
            <div class=""settlement-screen"" id=""settlementScreen"">
                <canvas class=""matrix-canvas"" id=""matrixCanvas""></canvas>
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
        let countdownText = document.getElementById('countdownText');
        let matrixCanvas = document.getElementById('matrixCanvas');
        let ctx = matrixCanvas.getContext('2d');

        let currentVideoUrl = null;
        let currentBlobUrl = null;
        let isVideoLoaded = false;
        let matrixAnimationId = null;
        let countdownInterval = null;
        let countdownSeconds = 0;
        let currentXHR = null;

        class MatrixRain {
            constructor(canvas) {
                this.canvas = canvas;
                this.ctx = canvas.getContext('2d');
                this.chars = '01';
                this.drops = [];
                this.animationId = null;
                
                this.resize();
                window.addEventListener('resize', () => this.resize());
            }
            
            resize() {
                this.canvas.width = window.innerWidth;
                this.canvas.height = window.innerHeight;
                
                const fontSize = 14;
                this.columns = Math.floor(this.canvas.width / fontSize);
                this.fontSize = fontSize;
                
                this.initializeDrops();
            }
            
            initializeDrops() {
                this.drops = [];
                for (let i = 0; i < this.columns; i++) {
                    this.drops[i] = Math.floor(Math.random() * -100);
                }
            }
            
            draw() {
                this.ctx.fillStyle = 'rgba(0, 0, 0, 0.04)';
                this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
                
                this.ctx.font = `${this.fontSize}px 'Courier New', monospace`;
                
                for (let i = 0; i < this.drops.length; i++) {
                    const text = this.chars[Math.floor(Math.random() * this.chars.length)];
                    const x = i * this.fontSize;
                    const y = this.drops[i] * this.fontSize;
                    
                    if (y < this.canvas.height * 0.3) {
                        this.ctx.fillStyle = '#0f0';
                    } else if (y < this.canvas.height * 0.6) {
                        this.ctx.fillStyle = '#0a0';
                    } else {
                        this.ctx.fillStyle = '#050';
                    }
                    
                    this.ctx.fillText(text, x, y);
                    
                    if (y > this.canvas.height && Math.random() > 0.975) {
                        this.drops[i] = 0;
                    }
                    
                    this.drops[i]++;
                }
            }
            
            start() {
                this.stop();
                const animate = () => {
                    this.draw();
                    this.animationId = requestAnimationFrame(animate);
                };
                animate();
            }
            
            stop() {
                if (this.animationId) {
                    cancelAnimationFrame(this.animationId);
                    this.animationId = null;
                }
                this.ctx.fillStyle = '#000';
                this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
            }
        }

        const matrixRain = new MatrixRain(matrixCanvas);

        function cleanupBlobUrl() {
            if (currentBlobUrl) {
                URL.revokeObjectURL(currentBlobUrl);
                currentBlobUrl = null;
            }
        }

        function cancelCurrentDownload() {
            if (currentXHR) {
                currentXHR.abort();
                currentXHR = null;
            }
        }

        function showLoadingScreen() {
            videoElement.style.display = 'none';
            loadingScreen.style.display = 'flex';
            settlementScreen.style.display = 'none';
            hideCountdown();
        }

        function hideLoadingScreen() {
            loadingScreen.style.display = 'none';
            videoElement.style.display = 'block';
            hideCountdown();
        }

        function showVideo() {
            videoElement.style.display = 'block';
            loadingScreen.style.display = 'none';
            settlementScreen.style.display = 'none';
            hideCountdown();
        }

        function showCountdown(seconds,errorMessage) {
            countdownSeconds = seconds;
            countdownText.style.display = 'block';
            updateCountdownText();
            const loadingTitle = document.querySelector('.loading-title');
            if (loadingTitle) {
                loadingTitle.style.display = 'none';
            }
            
            if (countdownInterval) {
                clearInterval(countdownInterval);
            }
            
            countdownInterval = setInterval(() => {
                countdownSeconds--;
                updateCountdownText();
                
                if (countdownSeconds <= 0) {
                    if (loadingTitle) {
                        loadingTitle.style.display = 'block';
                    }
                    hideCountdown();
                    showLoadingScreen();
                    notifyWPF('videoTimeout', {
                        videoUrl: currentVideoUrl,
                        error: errorMessage
                    });
                }
            }, 1000);
        }

        function hideCountdown() {
            if (countdownInterval) {
                clearInterval(countdownInterval);
                countdownInterval = null;
            }
            countdownText.style.display = 'none';
        }

        function updateCountdownText() {
            countdownText.textContent = `${countdownSeconds}秒后切换任务…`;
        }

        function initializePlayer() {
            videoElement.removeAttribute('controls');
            videoElement.disablePictureInPicture = true;
            videoElement.preload = ""auto"";
            videoElement.playbackRate = 1.0;
        
            showLoadingScreen();
        
            window.chrome.webview.addEventListener('message', event => {
                try {
                    const data = JSON.parse(event.data);
                    if (data.action === 'loadVideo') {
                        console.log('收到加载视频命令:', data.videoUrl);
                        if (data.videoUrl) {
                            loadAndPlayVideo(data.videoUrl);
                        }else{
                            showCountdown(10,'地址不正确');
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

        function preloadVideoWithBlob(videoUrl) {
            return new Promise((resolve, reject) => {
                currentXHR = new XMLHttpRequest();
                currentXHR.open('GET', videoUrl, true);
                currentXHR.responseType = 'blob';
                
                currentXHR.onload = function() {
                    if (currentXHR.status === 200) {
                        const blob = currentXHR.response;
                        const blobUrl = URL.createObjectURL(blob);
                        resolve(blobUrl);
                    } else {
                        reject(new Error('下载失败'));
                    }
                    currentXHR = null;
                };
                
                currentXHR.onerror = function() {
                    reject(new Error('网络错误'));
                    currentXHR = null;
                };
                
                currentXHR.ontimeout = function() {
                    reject(new Error('下载超时'));
                    currentXHR = null;
                };
                
                currentXHR.timeout = 60000;
                currentXHR.send();
            });
        }
    
        async function loadAndPlayVideo(videoUrl) {
            if (!videoUrl) {
                return;
            }
        
            currentVideoUrl = videoUrl;
            isVideoLoaded = false;

            try {
                showLoadingScreen();
                
                matrixRain.stop();
                hideCountdown();
                
                cleanupBlobUrl();
                cancelCurrentDownload();
                
                videoElement.pause();
                videoElement.src = '';
                videoElement.load();
                
                // 使用Blob完整下载
                const blobUrl = await preloadVideoWithBlob(videoUrl);
                currentBlobUrl = blobUrl;
                
                videoElement.src = blobUrl;

                await new Promise((resolve, reject) => {
                    let hasResolved = false;
                    
                    const onCanPlay = () => {
                        if (!hasResolved) {
                            hasResolved = true;
                            videoElement.removeEventListener('canplaythrough', onCanPlay);
                            videoElement.removeEventListener('error', onError);
                            resolve();
                        }
                    };
                    
                    const onError = (e) => {
                        if (!hasResolved) {
                            hasResolved = true;
                            videoElement.removeEventListener('canplaythrough', onCanPlay);
                            videoElement.removeEventListener('error', onError);
                            reject(new Error('视频加载错误'));
                        }
                    };
                    
                    videoElement.addEventListener('canplaythrough', onCanPlay);
                    videoElement.addEventListener('error', onError);
                    
                    if (videoElement.readyState >= 4) {
                        onCanPlay();
                    }
                });
                
                try {
                    await videoElement.play();
                    showVideo();
                    isVideoLoaded = true;
                    
                    notifyWPF('videoReady', {
                        videoUrl: currentVideoUrl
                    });
                    
                } catch (playError) {
                    videoElement.controls = true;
                    showVideo();
                    
                    notifyWPF('videoReady', {
                        videoUrl: currentVideoUrl,
                        requiresUserInteraction: true
                    });
                }

                videoElement.onended = () => {
                    cleanupBlobUrl();
                    showSettlementScreen();
                };

            } catch (error) {
                handleLoadError(error.message);
            } finally {
               
            }
        }

        function handleLoadError(errorMessage) {
            showLoadingScreen();
            isVideoLoaded = false;
            showCountdown(10,errorMessage);
        }
    
        function showSettlementScreen() {
            videoElement.style.display = 'none';
            loadingScreen.style.display = 'none';
            settlementScreen.style.display = 'flex';
            hideCountdown();
        
            matrixRain.start();
        
            notifyWPF('settlementComplete', {
                videoUrl: currentVideoUrl
            });
        }
    
        function settlementResult(success) {
            matrixRain.stop();
            
            if (success) {
                settlementScreen.style.display = 'none';
                showLoadingScreen();
                notifyWPF('VideoEnd', {
                    videoUrl: currentVideoUrl
                });
            } else {
                setTimeout(() => {
                    settlementScreen.style.display = 'none';
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
                window.chrome.webview.postMessage(JSON.stringify(message));
            }
        }
    
        window.addEventListener('DOMContentLoaded', () => {
            initializePlayer();
            setTimeout(() => {
                notifyWPF('pageReady', {});
            }, 100);
        });
        document.addEventListener('contextmenu', function(e) {
                e.preventDefault();
                return false;
            });
        document.addEventListener('keydown', function(e) {
                // 禁用F12、Ctrl+Shift+I等开发者工具快捷键
                if (e.key === 'F12' || 
                    (e.ctrlKey && e.shiftKey && e.key === 'I') ||
                    (e.ctrlKey && e.shiftKey && e.key === 'J') ||
                    (e.ctrlKey && e.key === 'u')) {
                    e.preventDefault();
                    return false;
                }
            });


        document.addEventListener('visibilitychange', function() {
            if (document.hidden) {
                matrixRain.stop();
                if (!videoElement.paused) {
                    videoElement.pause();
                }
            } else if (settlementScreen.style.display === 'flex') {
                matrixRain.start();
            }
        });

        window.addEventListener('beforeunload', () => {
            cleanupBlobUrl();
            cancelCurrentDownload();
        });
    </script>
</body>
</html>";

    }
}