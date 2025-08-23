using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Auxiliary.Elves.Client.Models;
using Microsoft.Web.WebView2.Core;

namespace Auxiliary.Elves.Client.Views
{
    public partial class SessionView : Window
    {
        private bool isVideoEnded = false;

        public SessionView()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            string userDataFolder = System.IO.Path.Combine(Environment.CurrentDirectory, "UserData", Guid.NewGuid().ToString());
            var env = await CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);
            await webView.EnsureCoreWebView2Async(env);
            webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;

            // 监听控制事件
            webView.CoreWebView2.AddHostObjectToScript("controller", new VideoController(this));

            PlayVideo(new VideoItem
            {
                Title = "测试",
                Url = "https://media.w3.org/2010/05/sintel/trailer.mp4"
            });
        }

        public void PlayVideo(VideoItem video)
        {
            txtTitle.Text = video.Title;
            txtBottom.Text = $"正在播放: {video.Title}";

            // 构建HTML视频播放器
            string htmlContent = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ margin: 0; padding: 0; background-color: black; }}
                    video {{ width: 100%; height: 100%; }}
                </style>
            </head>
            <body>
                <video id='videoPlayer' controls autoplay>
                    <source src='{video.Url}' type='video/mp4'>
                    您的浏览器不支持HTML5视频播放
                </video>
                <script>
                    const video = document.getElementById('videoPlayer');
                    
                    video.addEventListener('ended', function() {{
                        window.chrome.webview.hostObjects.controller.VideoEnded();
                    }});
                    
                    video.addEventListener('play', function() {{
                        window.chrome.webview.hostObjects.controller.VideoPlaying();
                    }});
                    
                    video.addEventListener('pause', function() {{
                        window.chrome.webview.hostObjects.controller.VideoPaused();
                    }});
                </script>
            </body>
            </html>";

            webView.NavigateToString(htmlContent);
        }

        public void Play()
        {
            webView.ExecuteScriptAsync("document.getElementById('videoPlayer').play();");
            txtBottom.Text = "播放中";
        }

        public void Stop()
        {
            webView.ExecuteScriptAsync("document.getElementById('videoPlayer').pause();");
            txtBottom.Text = "已暂停";
        }

        private void webView_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            // 可以添加源改变时的处理逻辑
        }

        private void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            // 导航完成时的处理
        }

        public void OnVideoEnded()
        {
            isVideoEnded = true;
            txtBottom.Text = "播放完成，显示结算动画";

            // 显示结算动画
            ShowSettlementAnimation();
        }

        private void ShowSettlementAnimation()
        {
            animationOverlay.Visibility = Visibility.Visible;
            animationCanvas.Children.Clear();

            // 创建动画元素
            var textBlock = new TextBlock
            {
                Text = "任务完成!",
                Foreground = Brushes.White,
                FontSize = 32,
                FontWeight = FontWeights.Bold
            };

            Canvas.SetLeft(textBlock, 200);
            Canvas.SetTop(textBlock, 200);
            animationCanvas.Children.Add(textBlock);

            // 创建星星动画
            for (int i = 0; i < 10; i++)
            {
                var star = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Yellow
                };

                Canvas.SetLeft(star, new Random().Next(100, 500));
                Canvas.SetTop(star, new Random().Next(100, 300));
                animationCanvas.Children.Add(star);

                // 星星动画
                DoubleAnimation opacityAnim = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };

                star.BeginAnimation(Ellipse.OpacityProperty, opacityAnim);
            }

            // 3秒后隐藏动画并播放下一个视频
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                animationOverlay.Visibility = Visibility.Collapsed;
                isVideoEnded = false;

                // 通知主窗口播放下一个视频
                //((MainWindow)Application.Current.MainWindow).PlayNextVideo();
            };
            timer.Start();
        }
    }

    // 用于WebView2和WPF之间的通信
    public class VideoController
    {
        private SessionView window;

        public VideoController(SessionView window)
        {
            this.window = window;
        }

        public void VideoEnded()
        {
            window.Dispatcher.Invoke(() => window.OnVideoEnded());
        }

        public void VideoPlaying()
        {
            // 可以添加播放状态更新
        }

        public void VideoPaused()
        {
            // 可以添加暂停状态更新
        }
    }
}