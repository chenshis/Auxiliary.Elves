using Auxiliary.Elves.Client.Models;
using Auxiliary.Elves.Client.Views;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Net.Http;
using Auxiliary.Elves.Api.Dtos;
using System.Security.Principal;
using HandyControl.Controls;

namespace Auxiliary.Elves.Client.ViewModels
{
    /// <summary>
    /// 主体视图模型
    /// </summary>
    public class MainViewModel : BindableBase
    {
        private bool _hasData = false;

        /// <summary>
        /// 验证是否有数据
        /// </summary>
        public bool HasData
        {
            get
            {
                return _hasData;
            }
            set
            {
                SetProperty(ref _hasData, value);
            }
        }

        private string _announcement;
        public string Announcement
        {
            get
            {
                return _announcement;
            }
            set
            {
                SetProperty(ref _announcement, value);
            }
        }

        private IWebHost _webHost;
        private int _port = 9527;
        private readonly IWindowService _windowService;
        private readonly IDialogService _dialogService;
        private readonly ILogger<MainViewModel> _logger;
        private readonly AuxElvesHttpClient _httpClient;

        public ObservableCollection<AccountModel> Accounts { get; set; }

        private Dictionary<AccountModel, SessionView> SessionViews { get; set; }

        public MainViewModel(IWindowService windowService, IDialogService dialogService,
            ILogger<MainViewModel> logger, AuxElvesHttpClient httpClient)
        {
            this._windowService = windowService;
            this._dialogService = dialogService;
            this._logger = logger;
            this._httpClient = httpClient;
            Accounts = new ObservableCollection<AccountModel>();
            SessionViews = new Dictionary<AccountModel, SessionView>();
        }

        private async Task StartHost()
        {
            try
            {
                // 创建并启动 WebHost
                _webHost = WebHost.CreateDefaultBuilder()
                    .UseUrls($"http://localhost:{_port}")
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Warning);
                    })
                    .ConfigureServices(services =>
                    {
                        // 可以在这里添加服务配置
                    })
                    .Configure(app =>
                    {

                        var baseDic = AppDomain.CurrentDomain.BaseDirectory;
                        var videoDirectory = Path.Combine(baseDic, "videos");
                        if (!Directory.Exists(videoDirectory))
                        {
                            Directory.CreateDirectory(videoDirectory);
                        }

                        _logger.LogInformation($"print video server address:{videoDirectory}");
                        // 启用静态文件服务
                        app.UseStaticFiles(new StaticFileOptions
                        {
                            FileProvider = new PhysicalFileProvider(videoDirectory),
                            RequestPath = "",
                            ServeUnknownFileTypes = true,
                            OnPrepareResponse = ctx =>
                            {
                                var fileExtension = Path.GetExtension(ctx.File.Name).ToLower();
                                ctx.Context.Response.Headers["Content-Type"] = GetMimeType(fileExtension);
                                ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                                ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=3600";
                            }
                        });

                        // 处理默认请求
                        app.Run(async context =>
                        {
                            if (context.Request.Path == "/")
                            {
                                await Task.CompletedTask;
                            }
                            else
                            {
                                context.Response.StatusCode = 404;
                                await context.Response.WriteAsync("File not found");
                            }
                        });
                    })
                    .Build();

                // 启动主机
                await _webHost.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                MessageBox.Show($"文件服务启动失败: {ex.Message}");
            }
        }
        private string GetMimeType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".mp4" => "video/mp4",
                ".webm" => "video/webm",
                ".ogg" => "video/ogg",
                ".avi" => "video/x-msvideo",
                ".mov" => "video/quicktime",
                ".wmv" => "video/x-ms-wmv",
                ".mkv" => "video/x-matroska",
                _ => "application/octet-stream"
            };
        }

        /// <summary>
        /// 数据擦汗寻
        /// </summary>
        private async Task DataQuery()
        {
            string mac = _logger.GetMac();
            if (mac == null)
            {
                return;
            }
            var apiResponse = await _httpClient.PostAsync<List<UserDto>>(
                 string.Concat(SystemConstant.UserMacRoute, $"?mac={mac}"));
            if (apiResponse == null)
            {
                _logger.LogError($"用户列表无响应");
                return;
            }
            if (apiResponse.Code == 1 || apiResponse.Data == null)
            {
                _logger.LogError("用户列表服务异常");
                return;
            }

            foreach (var item in apiResponse.Data)
            {
                var filterData = Accounts
                    .Where(t => t.AccountId == item.Userid && t.BindAccount == item.Userkey)
                    .Count();
                if (filterData > 0)
                {
                    continue;
                }
                var account = new AccountModel()
                {
                    AccountId = item.Userid,
                    BindAccount = item.Userkeyid,
                    ExpireTime = item.ExpireDate,
                    Status = true
                };
                Accounts.Add(account);
                SessionViews[account] = (SessionView)_windowService.ShowWindow<SessionViewModel, AccountModel>(account);

            }
            if (Accounts != null && Accounts.Count > 0)
            {
                HasData = true;
            }
        }

        private async Task GetAnnouncement()
        {
            var apiResponse = await _httpClient.PostAsync<List<AnnouncementDto>>(SystemConstant.AnnouncementRoute);
            if (apiResponse == null)
            {
                _logger.LogError($"公告无响应");
                Growl.Warning("获取公告失败，服务无响应");
                return;
            }
            if (apiResponse.Code == 1 || apiResponse.Data == null)
            {
                _logger.LogError("公告获取服务异常");
                Growl.Warning("请检查网络连接是否正常");
                return;
            }
            Announcement = string.Join("；", apiResponse.Data.Select(t => t.Announcement));
        }

        public ICommand ToggleCommand
        {
            get => new DelegateCommand<AccountModel>((m) => Toggle(m));
        }

        private void Toggle(AccountModel m)
        {
            m.Status = !m.Status;
            if (m.Status)
            {
                SessionViews[m].Start();
            }
            else
            {
                SessionViews[m].Stop();
            }
        }

        public ICommand DeleteCommand
        {
            get => new DelegateCommand<AccountModel>(async (m) => await Delete(m));
        }

        private async Task Delete(AccountModel m)
        {
            var apiResponse = await _httpClient.PostAsync<bool>(
                  string.Concat(SystemConstant.DelUserRoute, $"?userkeyidserId={m.BindAccount}"));
            if (apiResponse == null)
            {
                _logger.LogError($"删除账号无响应");
                Growl.Warning("删除账号失败，网络无响应");
                return;
            }
            if (apiResponse.Code == 1 || apiResponse?.Data == null)
            {
                _logger.LogError("删除账户服务异常");
                Growl.Warning("请检查网络连接是否正常");
                return;
            }
            SessionViews[m].Close();
            SessionViews.Remove(m);
            Accounts.Remove(m);
            if (Accounts == null || Accounts.Count() <= 0)
            {
                HasData = false;
            }
        }

        public ICommand ArrangeKeysCommand
        {
            get => new DelegateCommand(SetArrangeKeys);
        }

        private void SetArrangeKeys()
        {
            // 获取主屏幕的工作区域（排除任务栏）
            var workingArea = System.Windows.SystemParameters.WorkArea;

            // 计算网格布局
            int columns = (int)Math.Floor(workingArea.Width / 328);
            if (columns <= 0) columns = 1;

            int i = 0;
            foreach (var item in SessionViews)
            {
                int row = i / columns;
                int col = i % columns;

                double left = workingArea.Left + col * 328;
                double top = workingArea.Top + row * 428;

                // 确保窗口不会超出屏幕右边界
                if (left + 328 > workingArea.Right)
                {
                    left = workingArea.Right - 328;
                }

                // 确保窗口不会超出屏幕底部
                if (top + 428 > workingArea.Bottom)
                {
                    top = workingArea.Bottom - 428;
                }


                item.Value.Left = left;
                item.Value.Top = top;
                item.Value.WindowState = System.Windows.WindowState.Normal;


                i++;
            }
        }

        public ICommand AddAccountCommand
        {
            get => new DelegateCommand(SetAccount);
        }

        private void SetAccount()
        {
            IsEnable = false;
            _dialogService.ShowDialog(
                 nameof(AddUserDialogView),
                 async result =>
                 {
                     if (result.Result == ButtonResult.OK)
                     {
                         await DataQuery();
                     }
                 });

            IsEnable = true;
        }


        private bool _isEnable = true;


        /// <summary>
        /// 是否启用命令
        /// </summary>
        public bool IsEnable
        {
            get { return _isEnable; }
            set { SetProperty(ref _isEnable, value); }
        }


        public ICommand WindowClosingCommand
        {
            get => new DelegateCommand<CancelEventArgs>((e) =>
            {
                if (SessionViews != null && SessionViews.Count > 0)
                {
                    foreach (var item in SessionViews)
                    {
                        item.Value.Close();
                    }
                    SessionViews.Clear();
                }
                // 优雅关闭服务器
                _webHost?.StopAsync().Wait(3000);
                _webHost?.Dispose();
            });
        }

        public ICommand WindowClosedCommand
        {
            get => new DelegateCommand<EventArgs>((e) =>
            {

            });
        }

        public ICommand OnLoadedCommand
        {
            get => new DelegateCommand(async () =>
            {
                await GetAnnouncement();
                await StartHost();
                await DataQuery();
            });
        }

    }
}
