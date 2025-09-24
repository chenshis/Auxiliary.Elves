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
        /// 登录
        /// </summary>
        /// <param name="username">账号</param>
        /// <param name="password">卡密</param>
        /// <returns></returns>
        bool Login(AccountRequestDto accountRequest);

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userFeatureCode">特征码</param>
        /// <returns></returns>
        AccountUserDto Register(string userFeatureCode);

        /// <summary>
        /// 生成卡密
        /// </summary>
        /// <param name="userId">谷歌秘钥</param>
        /// <param name="verCode">验证码</param>
        /// <returns></returns>
        bool RegisterKey(string userId,string verCode);

        /// <summary>
        /// 根据账号查询卡密信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        List<UserDto> GetAllUser(string userName, bool enabled);

        /// <summary>
        /// 根据mac查询所有卡密
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        List<UserDto> GetMacAllUser(string mac);

        /// <summary>
        /// 通过谷歌账号找回账号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string RecoverAccount(string userId, string verCode);

        /// <summary>
        /// 根据卡账号删除
        /// </summary>
        /// <param name="Userkeyid"></param>
        /// <returns></returns>
        bool DeleteUser(string userkeyid);
    }
}
