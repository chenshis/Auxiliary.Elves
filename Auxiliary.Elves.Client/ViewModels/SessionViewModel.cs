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
        public readonly ILogger<SessionViewModel> _logger;

        public SessionViewModel(ILogger<SessionViewModel> logger)
        {
            this._logger = logger;
        }

        public void ApplyParameters<AccountModel>(AccountModel parameter)
        {
        }
    }
}
