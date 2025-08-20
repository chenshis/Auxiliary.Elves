using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.IApiService
{
    public interface ILoginApiService
    {

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userFeatureCode">特征码</param>
        /// <returns></returns>
        bool Register(string userFeatureCode);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">账号</param>
        /// <param name="password">卡密</param>
        /// <returns></returns>
        ApiResponse<string> Login(AccountRequestDto accountRequest);

    }
}
