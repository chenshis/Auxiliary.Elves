using Auxiliary.Elves.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Domain.Entities
{
    [Table("sys_userkey")]
    public class UserKeyEntity : BizEntityBase
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
        /// 卡密绑定IP,;
        /// </summary>
        public string? Userkeyip { get; set; }

        /// <summary>
        /// 绑定谷歌账号,;
        /// </summary>
        public string? Userkeybindaccount { get; set; }

        /// <summary>
        /// 最后登录日期,;
        /// </summary>
        public DateTime? Userkeylastdate { get; set; }

        /// <summary>
        /// 是否在线,;
        /// </summary>
        public bool Isonline { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLock { get; set; }

        /// <summary>
        /// 是否运行
        /// </summary>
        public bool IsRun { get; set; } 
    }
}
