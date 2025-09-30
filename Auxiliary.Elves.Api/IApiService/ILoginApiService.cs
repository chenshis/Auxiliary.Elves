using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Domain.Entities;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.IApiService
{
    public interface ILoginApiService
    {

        /// <summary>
        /// 根据账号查询所有被邀请用户
        /// </summary>
        /// <param name="userName">账号</param>
        /// <returns></returns>
        List<AccountUserDto> GetUserInviteUserInfo(string userName);

        /// <summary>
        /// 根据账号修改被邀请人
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userInviteUserName">邀请人</param>
        /// <returns></returns>
        bool SetUserInvite(string userName, string userInviteUserName);
        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        List<AccountUserDto> GetAllUser();
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
        /// <param name="userFeatureCode">邀请人账号</param>
        /// <returns></returns>
        bool Register(string userFeatureCode, string userInviteUserName);

        /// <summary>
        /// 绑定谷歌
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userId">谷歌账号</param>
        /// <returns></returns>
        bool BindGoogle(string userName, string userId);


        /// <summary>
        /// 用户设置是否有效
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userId">谷歌密钥</param>
        /// <returns></returns>
        bool SetEnableStatus(string userName, bool isEnable);

        /// <summary>
        /// 生成卡密
        /// </summary>
        /// <param name="userId">谷歌秘钥</param>
        /// <param name="verCode">验证码</param>
        /// <returns></returns>
        bool RegisterKey(string userId,string verCode);

        /// <summary>
        /// 获取所有用户卡密信息
        /// </summary>
        /// <returns></returns>
        List<UserDto> GetAllUserKey();

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
