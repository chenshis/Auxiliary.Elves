using Auxiliary.Elves.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Domain.Entities
{
    [Table("sys_systemsetting")]
    public class SystemSettingEntity : BizEntityBase
    {
        /// <summary>
        /// 手续费,;
        /// </summary>
        public int Handlingfee { get; set; }

        /// <summary>
        /// 最低数量,;
        /// </summary>
        public int Minnumber { get; set; }

        /// <summary>
        /// 最小区间,;
        /// </summary>
        public int Mininterval { get; set; }

        /// <summary>
        /// 最大区间,;
        /// </summary>
        public int Maxinterval { get; set; }
    }
}
