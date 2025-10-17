using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Infrastructure.Config
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 获取当前北京时间（自动判断本地时区）
        /// </summary>
        public static DateTime Now
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(
                            DateTime.UtcNow,
                            TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")
                        ); 
            }
        }
    }
}
