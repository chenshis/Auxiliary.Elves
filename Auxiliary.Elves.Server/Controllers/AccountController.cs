using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.AspNetCore.Mvc;
using NLog.Web.LayoutRenderers;

namespace Auxiliary.Elves.Server.Controllers
{
    public class AccountController : AuxiliaryControllerBase
    {
        public ILoginApiService LoginApiService { get; }

        public AccountController(ILoginApiService loginApiService)
        {
            LoginApiService = loginApiService;
        }

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
        public bool SetEnableStatus(string userName, bool isEnable)
        {
            return LoginApiService.SetEnableStatus(userName, isEnable);
        }

        
        /// <summary>
        /// 生成卡密
        /// </summary>
        /// <param name="userId">谷歌秘钥</param>
        /// <param name="verCode">验证码</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.RegisterKeyRoute)]
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
        public List<UserDto> GetMacAllUser(string mac)
        {
            return LoginApiService.GetMacAllUser(mac);
        }

      
    }
}
