using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.Dtos
{
    public class VideoDto
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string VideoUrl { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string VideoExpireDate { get; set; } 
    }
}
