using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain.Entities;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auxiliary.Elves.Server.Controllers
{
    public class SystemSettingController : AuxiliaryControllerBase
    {
        public ISystemSettingApiService  SystemSettingApiService { get; }

        public SystemSettingController(ISystemSettingApiService  systemSettingApiService)
        {
            SystemSettingApiService = systemSettingApiService;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="settingRequestDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.SetSystemSettingRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public bool SetSystemSetting(SystemSettingRequestDto settingRequestDto)
        {
            return SystemSettingApiService.SetSystemSetting(settingRequestDto);
        }

        /// <summary>
        /// 查询参数
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.SystemSettingRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public SystemSettingRequestDto GetSystemSetting() 
        {
          return SystemSettingApiService.GetSystemSetting();
        }
    }
}
