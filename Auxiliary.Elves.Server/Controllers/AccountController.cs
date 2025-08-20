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
        /// 用户注册
        /// </summary>
        /// <param name="userFeatureCode">特征码</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.RegisterRoute)]
        public bool Register([FromBody] string userFeatureCode)
        {
            return LoginApiService.Register(userFeatureCode);
        }
    }
}
