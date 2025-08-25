using Auxiliary.Elves.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.IApiService
{
    public interface ISystemSettingApiService
    {
        /// <summary>
        /// 设置公告
        /// </summary>
        /// <param name="settingRequestDto"></param>
        /// <returns></returns>
        public bool SetSystemSetting(SystemSettingRequestDto settingRequestDto);

        /// <summary>
        /// 查询设置
        /// </summary>
        /// <returns></returns>
        public SystemSettingRequestDto GetSystemSetting();
    }
}
