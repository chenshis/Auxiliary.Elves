using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.AspNetCore.Mvc;

namespace Auxiliary.Elves.Server.Controllers
{

    public class PointsController : AuxiliaryControllerBase
    {
        public IPointsApiService PointsApiService { get; }

        public PointsController(IPointsApiService pointsApiService)
        {
            PointsApiService = pointsApiService;
        }

        /// <summary>
        /// 新增积分
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="points">积分</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.AddPointsRoute)]
        public bool AddPoints(string userName, int points)
        {
            return PointsApiService.AddPoints(userName, points);
        }

        /// <summary>
        /// 获取积分
        /// </summary>
        /// <param name="userName">账号</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.PointsRoute)]
        public PointsDto GetPoints(string userName) 
        {
            return PointsApiService.GetPoints(userName);
        }

        /// <summary>
        /// 提取积分
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="points">提取积分</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.ExtractRoute)]
        public bool ExtractPoints(string userName, int points)
        {
            return PointsApiService.ExtractPoints(userName, points);
        }

        /// <summary>
        /// 查询提取记录
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.UserPointsRecord)]
        public List<PointsDto> GetExtractPoints(string userName)
        {
            return PointsApiService.GetExtractPoints(userName);
        }   
    }
}
