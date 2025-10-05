using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain.Entities;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public bool AddPoints(string userName)
        {
            return PointsApiService.AddPoints(userName, 1);
        }

        /// <summary>
        /// 提取积分
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="points">提取积分</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.ExtractRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public bool ExtractPoints(string userName, int points)
        {
            return PointsApiService.ExtractPoints(userName, points);
        }


        /// <summary>
        /// 根据账号获取总积分
        /// </summary>
        /// <param name="userName">账号</param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.PointsRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public PointsDto GetPoints(string userName) 
        {
            return PointsApiService.GetPoints(userName);
        }


        /// <summary>
        /// 获取所有账号总积分
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.PointsUserRoute)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public List<PointsDto> GetPointsUser()
        {
            return PointsApiService.GetPointsUser();
        }

        /// <summary>
        /// 获取所有账号积分记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.UserPointsRecord)]
        [Authorize(Roles = nameof(RoleEnum.Admin))]
        public List<PointsDto> GetRecordPoints()
        {
            return PointsApiService.GetRecordPoints();
        }

      
    }
}
