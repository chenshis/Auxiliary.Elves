using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.Dtos
{
    public class SystemSettingRequestDto
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
