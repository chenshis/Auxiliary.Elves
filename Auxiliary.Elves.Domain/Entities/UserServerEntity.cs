using Auxiliary.Elves.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Domain.Entities
{
    [Table("sys_serveruser")]
    public class UserServerEntity : BizEntityBase
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 预留 暂时都是1 无任何作用
        /// </summary>
        public RoleEnum Role { get; set; }

        /// <summary>
        /// jwt id 用于刷新token
        /// </summary>
        public string Jti { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get;set; }

    }

    /// <summary>
    /// 角色枚举
    /// </summary>
    [Flags]
    public enum RoleEnum
    {
        /// <summary>
        /// 管理员
        /// </summary>
        [Description("管理员")]
        Admin = 1
    }
}
