using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.AspNetCore.Mvc;

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
        public string Login([FromBody] AccountRequestDto accountRequest)
        {
            var user = LoginApiService.Login(accountRequest);
            return "";
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userFeatureCode">特征码</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.RegisterRoute)]
        public string Register([FromBody] string userFeatureCode)
        {
            return LoginApiService.Register(userFeatureCode);
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
    }
}
