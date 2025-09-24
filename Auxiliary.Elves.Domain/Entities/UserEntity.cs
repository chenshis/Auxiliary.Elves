using Auxiliary.Elves.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Domain.Entities
{

    [Table("sys_user")]
    public class UserEntity : BizEntityBase
    {
        /// <summary>
        /// 编码,;
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// 特征码,;
        /// </summary>
        public string Userfeaturecode { get; set; }

        /// <summary>
        /// 账号,;
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 备用数字,;
        /// </summary>
        public string Userbakckupnumber { get; set; }
    }
}
