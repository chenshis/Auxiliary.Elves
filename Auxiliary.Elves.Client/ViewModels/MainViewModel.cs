using Auxiliary.Elves.Client.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
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
        private readonly IWindowService _windowService;

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

        public ObservableCollection<AccountModel> Accounts { get; set; }

        public MainViewModel(IWindowService windowService)
        {
            this._windowService = windowService;
            Accounts = new ObservableCollection<AccountModel>();
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
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

        public ICommand StopCommand
        {
            get => new DelegateCommand<AccountModel>((m) => Stop(m));
        }

        private void Stop(AccountModel m)
        {
            _windowService.ShowWindow<SessionViewModel>();
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

        }


    }
}
