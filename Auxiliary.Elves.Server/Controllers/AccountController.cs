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
        public bool Login([FromBody] AccountRequestDto accountRequest)
        {
            return LoginApiService.Login(accountRequest);
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

        /// <summary>
        /// 根据账号查询卡密信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="enabled">是否启用</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.UserRoute)]
        public List<UserDto> GetAllUser(string userName,bool enabled)
        {
            return LoginApiService.GetAllUser(userName, enabled);
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
