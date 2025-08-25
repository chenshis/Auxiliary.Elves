using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.Dtos
{
    public class UserDto
    {
        /// <summary>
        /// 账号唯一编码,;
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// 卡密唯一编码,;
        /// </summary>
        public string Userkeyid { get; set; }

        /// <summary>
        /// 卡密,;
        /// </summary>
        public string Userkey { get; set; }

        /// <summary>
        /// 最后登录日期,;
        /// </summary>
        public string Userkeylastdate { get; set; }

        /// <summary>
        /// 是否在线,;
        /// </summary>
        public bool Isonline { get; set; }
    }
}
