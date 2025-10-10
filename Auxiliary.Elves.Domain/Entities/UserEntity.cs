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
        public string UserId { get; set; }

        /// <summary>
        /// 特征码,;
        /// </summary>
        public string UserFeatureCode { get; set; }

        /// <summary>
        /// 账号,;
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 备用数字,;
        /// </summary>
        public string UserBakckupNumber { get; set; }

        /// <summary>
        /// 绑定谷歌账号
        /// </summary>
        public string UserBindAccount { get; set; }

        /// <summary>
        /// 邀请人账号
        /// </summary>
        public string UserInviteUserName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string UserAddress { get; set; }
    }
}
