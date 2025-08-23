using Auxiliary.Elves.Client.Models;
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
        public SessionViewModel()
        {
                
        }

        public void ApplyParameters<AccountModel>(AccountModel parameter)
        {
        }
    }
}
