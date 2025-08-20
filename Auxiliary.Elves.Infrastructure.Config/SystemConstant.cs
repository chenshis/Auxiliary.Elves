using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Infrastructure.Config
{
    /// <summary>
    /// 系统常量
    /// </summary>
    public static class SystemConstant
    {
        /// <summary>
        /// 登录路由
        /// </summary>
        public const string LoginRoute = "v1/auxiliary/account/login";

        /// <summary>
        /// 注册路由
        /// </summary>
        public const string RegisterRoute = "v1/auxiliary/account/register";

        /// <summary>
        /// 默认连接
        /// </summary>
        public const string DefaultConnection = nameof(DefaultConnection);

        /// <summary>
        /// 端口
        /// </summary>
        public const string HostFileName = "host.json";

        /// <summary>
        /// 宿主地址列表
        /// </summary>
        public const string HostPort = "HostPort";
    }
}
