using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.Dtos
{
    public class AccountRequestDto
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 卡
        /// </summary>
        public string UserKeyId { get; set; }

        /// <summary>
        /// 卡密
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Mac
        /// </summary>
        public string Mac { get; set; }

    }
}
