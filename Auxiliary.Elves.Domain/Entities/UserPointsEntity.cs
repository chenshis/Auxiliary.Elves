using Auxiliary.Elves.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Domain.Entities
{
    [Table("sys_userpoints")]
    public class UserPointsEntity : BizEntityBase
    {

        /// <summary>
        /// 账号唯一编码,;
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// 积分,;
        /// </summary>
        public int Userpoints { get; set; }
    }
}
