using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Client.Models
{
    public class AccountModel : BindableBase
    {
        public string AccountId { get; set; }
        public string BindAccount { get; set; }
        public string ExpireTime { get; set; }

        private bool _status = false;

        /// <summary>
        /// 验证是否有数据
        /// </summary>
        public bool Status
        {
            get
            {
                return _status;
            }
            set
            {
                SetProperty(ref _status, value);
            }
        }
    }
}
