using Auxiliary.Elves.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auxiliary.Elves.Domain.Entities
{
    [Table("sys_userpointsrecord")]
    public class UserPointsRecordEntity : BizEntityBase
    {

        /// <summary>
        /// 账号唯一编码,;
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// 积分提取日期,;
        /// </summary>
        public DateTime Userdata { get; set; }

        /// <summary>
        /// 积分,;
        /// </summary>
        public int Userpoints { get; set; }
    }
}
