using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Infrastructure.Config;
using HandyControl.Controls;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Auxiliary.Elves.Client.ViewModels
{
    /// <summary>
    /// 添加用户视图模型
    /// </summary>
    public class AddUserDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "添加账户";

        public event Action<IDialogResult> RequestClose;

        public AddUserDialogViewModel(AuxElvesHttpClient httpClient, ILogger<AddUserDialogViewModel> logger)
        {
            this._httpClient = httpClient;
            this._logger = logger;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        /// <summary>
        /// 关闭用户窗口命令
        /// </summary>
        public ICommand CloseCommand
        {
            get => new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.OK)));
        }

        /// <summary>
        /// 取消命令
        /// </summary>
        public ICommand CancelCommand
        {
            get => new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
        }

        /// <summary>
        /// 执行成功窗口命令
        /// </summary>
        public ICommand SuccessCommand
        {
            get =>
                new DelegateCommand<IDialogParameters>((parameters) => RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters)));
        }

        private string _userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                SetProperty(ref _userName, value);
            }
        }

        private string _password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
            }
        }

        private string _contacts;
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contacts
        {
            get { return _contacts; }
            set { SetProperty(ref _contacts, value); }
        }



        private string _errorMessage;
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetProperty(ref _errorMessage, value); }
        }

        private bool _isEnable = true;
        private readonly AuxElvesHttpClient _httpClient;
        private readonly ILogger<AddUserDialogViewModel> _logger;

        /// <summary>
        /// 是否启用命令
        /// </summary>
        public bool IsEnable
        {
            get { return _isEnable; }
            set { SetProperty(ref _isEnable, value); }
        }


        public ICommand ConfirmCommand
        {
            get => new DelegateCommand<UserControl>
                (async u => await SetAddUserActive(u)).ObservesCanExecute(() => IsEnable);
        }

        private async Task SetAddUserActive(UserControl control)
        {
            IsEnable = false;
            HandyControl.Controls.PasswordBox userPwd = null;
            userPwd = control.FindName(nameof(userPwd)) as HandyControl.Controls.PasswordBox;
            Password = userPwd.Password;
            ErrorMessage = null;
            if (string.IsNullOrWhiteSpace(UserName))
            {
                ErrorMessage = "请输入账号";
                IsEnable = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "请输入密码";
                IsEnable = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(Contacts))
            {
                ErrorMessage = "请输入绑定账号";
                IsEnable = true;
                return;
            }
            var mac = _logger.GetMac();
            if (mac == null)
            {
                ErrorMessage = "获取mac地址失败";
                IsEnable = true;
                return;
            }
            var apiResponse = await _httpClient.PostAsync<AccountRequestDto, bool>(SystemConstant.LoginRoute, new AccountRequestDto
            {
                UserName = Contacts,
                Password = Password,
                UserKeyId = UserName,
                Mac = mac
            });
            if (apiResponse == null)
            {
                _logger.LogError($"添加账户无响应");
                ErrorMessage = "添加账户无响应";
                IsEnable = true;
                return;
            }
            if (apiResponse.Code == 1 || apiResponse?.Data == null)
            {
                _logger.LogError("添加账户服务异常");
                ErrorMessage = "请检查网络连接是否正常";
                IsEnable = true;
                return;
            }
            if (apiResponse.Data == false)
            {
                ErrorMessage = "请检查输入账户信息是否合法";
                IsEnable = true;
                return;
            }

            CloseCommand.Execute(ButtonResult.OK);
        }
    }
}
