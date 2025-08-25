using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.Dtos
{
    public class PointsDto
    {

        /// <summary>
        /// 积分,;
        /// </summary>
        public int Userpoints { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string UserPointsDate { get; set; }
    }
}
