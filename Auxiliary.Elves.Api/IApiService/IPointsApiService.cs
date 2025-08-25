using Auxiliary.Elves.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.IApiService
{
    public interface IPointsApiService
    {
        /// <summary>
        /// 查询提取记录
        /// </summary>
        /// <param name="userName">账号</param>
        /// <returns></returns>
        List<PointsDto> GetExtractPoints(string userName);

        /// <summary>
        /// 提取积分
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="points">积分</param>
        /// <returns></returns>
        bool ExtractPoints(string userName, int points);

        /// <summary>
        /// 新增积分
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="points">积分</param>
        /// <returns></returns>
        bool AddPoints(string userName, int points);

        /// <summary>
        /// 根据账号获取积分
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        PointsDto GetPoints(string userName);


    }
}
