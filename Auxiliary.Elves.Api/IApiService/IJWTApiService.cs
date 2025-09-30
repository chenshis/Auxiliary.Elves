using Auxiliary.Elves.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.IApiService
{
    public interface IJWTApiService
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        string GetToken(UserServerEntity userEntity);

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="oldToken"></param>
        /// <returns></returns>
        string RefreshToken(string oldToken);
    }
}
