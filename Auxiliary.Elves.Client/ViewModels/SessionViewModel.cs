using Auxiliary.Elves.Client.Models;
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
        public readonly ILogger<SessionViewModel> Logger;
        public AccountModel Account { get; set; }

        public SessionViewModel(ILogger<SessionViewModel> logger)
        {
            this.Logger = logger;
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
    }
}
