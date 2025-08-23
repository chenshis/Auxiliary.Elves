using Auxiliary.Elves.Client.Models;
using Auxiliary.Elves.Client.Views;
using Auxiliary.Elves.Infrastructure.Config;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public MainViewModel(IWindowService windowService, IDialogService dialogService)
        {
            this._windowService = windowService;
            this._dialogService = dialogService;
            Accounts = new ObservableCollection<AccountModel>();
            DataQuery();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void DataQuery()
        {
            for (int i = 0; i < 20; i++)
            {
                Accounts.Add(new AccountModel()
                {
                    Id = i + 1,
                    AccountId = $"Test{i + 1}",
                    BindAccount = new Random().Next(100000, 99999999).ToString(),
                    ExpireTime = DateTime.Now.AddDays(30 - i),
                    Status = i % 2 == 0 ? true : false
                });
            }
            HasData = true;
        }

        public ICommand ToggleCommand
        {
            get => new DelegateCommand<AccountModel>(async (m) => await Toggle(m));
        }

        private async Task Toggle(AccountModel m)
        {
            m.Status = !m.Status;
            _windowService.ShowWindow<SessionViewModel, AccountModel>(m);
            //VideoItem item = new VideoItem();
            //item.Title = m.AccountId;
            //item.Url = "https://media.w3.org/2010/05/sintel/trailer.mp4";
            //await window.PlayVideo(item);
        }

        public ICommand DeleteCommand
        {
            get => new DelegateCommand<AccountModel>((m) => Delete(m));
        }

        private void Delete(AccountModel m)
        {

        }

        public ICommand ArrangeKeysCommand
        {
            get => new DelegateCommand(SetArrangeKeys);
        }

        private void SetArrangeKeys()
        {

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
