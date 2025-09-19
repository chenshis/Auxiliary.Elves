using Auxiliary.Elves.Client.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Wpf;
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

        public AccountModel Account { get; set; }

        public SessionViewModel(ILogger<SessionViewModel> logger, AuxElvesHttpClient httpClient)
        {
            this._logger = logger;
            this._httpClient = httpClient;
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

        public string GetVideoAddress()
        {
            return "https://media.w3.org/2010/05/sintel/trailer.mp4";
        }

        public async Task<bool> UpdatePoints()
        {
            var userName = Account.AccountId;




            return await Task.Run(() => true);
        }
    }
}
