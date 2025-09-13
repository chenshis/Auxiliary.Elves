using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.Dtos
{
    public class AccountUserDto
    {
        /// <summary>
        /// 账号唯一编码,;
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }
    }
}
