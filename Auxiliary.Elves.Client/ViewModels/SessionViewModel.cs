using Auxiliary.Elves.Client.Models;
using Auxiliary.Elves.Infrastructure.Config;
using HandyControl.Controls;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Wpf;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Client.ViewModels
{
    public class SessionViewModel : BindableBase, IParameterReceiver
    {
        private readonly ILogger<SessionViewModel> _logger;
        private readonly AuxElvesHttpClient _httpClient;
        private readonly IEventAggregator _eventAggregator;

        public AccountModel Account { get; set; }

        public SessionViewModel(ILogger<SessionViewModel> logger, AuxElvesHttpClient httpClient, IEventAggregator eventAggregator)
        {
            this._logger = logger;
            this._httpClient = httpClient;
            this._eventAggregator = eventAggregator;
        }

        public void ExecutePublishMessage()
        {
            // 创建并发布消息
            var message = new StatusMessage { Message = Account, Timestamp = DateTime.Now };
            _eventAggregator.GetEvent<SubViewStatusEvent>().Publish(message);
        }

        public void RecordInfo(string message)
        {
            _logger.LogInformation(message);
        }

        public void RecordError(Exception ex, string message)
        {
            _logger.LogError(ex, message);
        }

        public void ApplyParameters<Model>(Model parameter)
        {
            if (parameter == null)
            {
                return;
            }
            if (parameter is AccountModel account)
            {
                Account = account;
            }
        }

        public async Task<string> GetVideoAddressAsync()
        {
            _logger.LogInformation($"{Account.AccountId}:拉取视频");
            var apiResponse = await _httpClient.PostAsync<string>(SystemConstant.VideoVideoUrlRoute);
            if (apiResponse == null)
            {
                return await Task.FromResult<string>(null);
            }
            if (apiResponse.Code == 1 || apiResponse.Data == null)
            {
                _logger.LogError($"{Account.BindAccount}:拉取视频服务异常");
                return await Task.FromResult<string>(null);
            }
            var originalString = apiResponse.Data;
            string result = originalString.Length >= 8 ?
                originalString.Substring(originalString.Length - 8) : originalString;
            _logger.LogInformation($"{Account.AccountId}:拉取视频({result})");
            return string.Concat(SystemConstant.ServerUrl, apiResponse.Data);
        }

        public async Task<bool> UpdatePoints()
        {

            var userName = Account.BindAccount;
            var apiResponse = await _httpClient.PostAsync<bool>(string.Concat(SystemConstant.AddPointsRoute, $"?userName={userName}"));

            if (apiResponse?.Data == false)
            {
                _logger.LogError($"{Account.AccountId}:更新积分失败");
                return false;
            }
            _logger.LogInformation($"{Account.AccountId}:更新积分成功");
            return true;
        }
    }
}
