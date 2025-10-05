using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain.Entities;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace Auxiliary.Elves.Server.Controllers
{
    public class AccountController : AuxiliaryControllerBase
    {
        public ILoginApiService LoginApiService { get; }
        private readonly IJWTApiService JWTApiService;

        public AccountController(ILoginApiService loginApiService,IJWTApiService jWTApiService)
        {
            LoginApiService = loginApiService;
            JWTApiService= jWTApiService;
        }


        /// <summary>
        /// 登录(别用，与客户端交互用)
        /// </summary>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.LoginKeyRoute)]
        public string LoginServer(string key)
        {
            if (key=="admin")
            {
                UserServerEntity user = new UserServerEntity()
                {
                    UserName = "dfasdfsfasfasf",
                    Password = "sddwwww",
                    Role= RoleEnum.Admin
                };

                return JWTApiService.GetToken(user);
            }
            return "";
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="accountRequest">账户信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.LoginServerRoute)]
        public string LoginServer([FromBody] UserRequestDto accountRequest)
        {
            var user = LoginApiService.LoginServer(accountRequest);
            return JWTApiService.GetToken(user);
        }


        /// <summary>
        /// token刷新
        /// </summary>
        /// <param name="token">原token</param>
        /// <param name="key">刷新密钥</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.RefreshTokenRoute)]
        public string RefreshToken([FromBody] string token,string key)
        {
            if (key != "Qm9kZUFJX0pXVF9TZWNyZXRfa2V5XzIwMjUtMDktMzBfSldUX1NhZmU=")
                return "";

            return JWTApiService.RefreshToken(token);
        }

        /// <summary>
        /// 注册服务器账号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.RegisterServerRoute)]
        public bool RegisterServer([FromBody] UserRequestDto request) => LoginApiService.AddUser(request);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="accountRequest">账户信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.LoginRoute)]
        public bool Login([FromBody] AccountRequestDto accountRequest)
        {
            return LoginApiService.Login(accountRequest);
        }

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.UserRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public List<AccountUserDto> GetAllUser()
        {
            return LoginApiService.GetAllUser();
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userFeatureCode">特征码</param>
        /// <param name="userInviteUserName">邀请人账号</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.RegisterRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public bool Register(string userFeatureCode,string userInviteUserName="")
        {
            return LoginApiService.Register(userFeatureCode, userInviteUserName);
        }

        /// <summary>
        /// 用户绑定谷歌
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userId">谷歌密钥</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.BindGoogleRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public bool BindGoogleRoute(string userName,string userId)
        {
            return LoginApiService.BindGoogle(userName, userId);
        }

        /// <summary>
        /// 用户设置是否有效
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userId">谷歌密钥</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.SetEnableStatusRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public bool SetEnableStatus(string userName, bool isEnable)
        {
            return LoginApiService.SetEnableStatus(userName, isEnable);
        }

        /// <summary>
        /// 根据账号查询所有被邀请用户
        /// </summary>
        /// <param name="userName">账号</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.UserInviteInfoRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public List<AccountUserDto> GetUserInviteUserInfo(string userName)
        {
            return LoginApiService.GetUserInviteUserInfo(userName);
        }


        /// <summary>
        /// 根据账号修改被邀请人
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userInviteUserName">邀请人</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.SetUserInviteRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public bool SetUserInvite(string userName, string userInviteUserName )
        {
            return LoginApiService.SetUserInvite(userName, userInviteUserName);
        }

        /// <summary>
        /// 生成卡密
        /// </summary>
        /// <param name="userId">谷歌秘钥</param>
        /// <param name="verCode">验证码</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.RegisterKeyRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public bool RegisterKey(string userId,string verCode)
        {
            return LoginApiService.RegisterKey(userId,verCode);
        }

        /// <summary>
        /// 获取所有用户卡信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.UserKeyRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public List<UserDto> GetAllUserKey()
        {
            return LoginApiService.GetAllUserKey();
        }

        /// <summary>
        /// 找回账号
        /// </summary>
        /// <param name="userId">谷歌秘钥</param>
        /// <param name="verCode">验证码</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.RecoverRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public string RecoverAccount(string userId, string verCode) 
        {
            return LoginApiService.RecoverAccount(userId,verCode);
        }


        /// <summary>
        /// 根据卡账号删除
        /// </summary>
        /// <param name="userkeyidserId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.DelUserRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public bool DelUser(string userkeyidserId)
        {
            return LoginApiService.DeleteUser(userkeyidserId);
        }

        /// <summary>
        /// 根据mac查找所有卡账号
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.UserMacRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public List<UserDto> GetMacAllUser(string mac)
        {
            return LoginApiService.GetMacAllUser(mac);
        }


        /// <summary>
        /// 根据账号查询下面所有卡密
        /// </summary>
        /// <param name="userId">账号</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.UserToKeyRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public List<UserDto> GetUserAllKey(string userId)
        {
            return LoginApiService.GetUserAllKey(userId);
        }
    }
}
