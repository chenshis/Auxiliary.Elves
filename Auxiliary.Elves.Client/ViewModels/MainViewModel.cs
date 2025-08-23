using Auxiliary.Elves.Client.Models;
using Auxiliary.Elves.Client.Views;
using Auxiliary.Elves.Infrastructure.Config;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;

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

        private readonly IWindowService _windowService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<AccountModel> Accounts { get; set; }

        private Dictionary<AccountModel, SessionView> SessionViews { get; set; }

        public MainViewModel(IWindowService windowService, IDialogService dialogService)
        {
            this._windowService = windowService;
            this._dialogService = dialogService;
            Accounts = new ObservableCollection<AccountModel>();
            SessionViews = new Dictionary<AccountModel, SessionView>();
            DataQuery();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void DataQuery()
        {
            for (int i = 0; i < 5; i++)
            {
                var account = new AccountModel()
                {
                    Id = i + 1,
                    AccountId = $"Test{i + 1}",
                    BindAccount = new Random().Next(100000, 99999999).ToString(),
                    ExpireTime = DateTime.Now.AddDays(30 - i),
                    Status = true
                };
                Accounts.Add(account);
                SessionViews[account] = (SessionView)_windowService.ShowWindow<SessionViewModel, AccountModel>(account);
            }
            HasData = true;
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
            get => new DelegateCommand<AccountModel>((m) => Delete(m));
        }

        private void Delete(AccountModel m)
        {
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
            var workingArea = SystemParameters.WorkArea;

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
                item.Value.WindowState = WindowState.Normal;


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
                result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        DataQuery();
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


    }
}
