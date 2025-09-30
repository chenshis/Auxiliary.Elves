using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain;
using Auxiliary.Elves.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.ApiService
{
    public class PointsApiService : IPointsApiService
    {
        private readonly AuxiliaryDbContext _dbContext;

        public PointsApiService(AuxiliaryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// 新增积分
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool AddPoints(string userName, int points)
        {
            if (string.IsNullOrWhiteSpace(userName) || points <= 0)
                return false;

            var userEntity = _dbContext.UserKeyEntities.FirstOrDefault(t => t.Userid == userName);

            if (userEntity == null)
                return false;

            var userPoints = _dbContext.UserPointsEntities.FirstOrDefault(t => t.Userid == userName);

            if (userPoints != null)
            {
                userPoints.Userpoints += SystemConstant.MaxPoints;
                userPoints.UpdateTime = DateTime.Now;
                _dbContext.UserPointsEntities.Update(userPoints);
            }
            else
            {
                var userModel = new Domain.Entities.UserPointsEntity
                {
                    Userid = userName,
                    Userpoints = SystemConstant.MaxPoints
                };
                 _dbContext.UserPointsEntities.Add(userModel);
            }

            //记录积分记录
            _dbContext.UserPointsRecordEntities.Add(new Domain.Entities.UserPointsRecordEntity
            {
                Userid = userName,
                UpdateTime = DateTime.Now,
                Userpoints= SystemConstant.MaxPoints,
                IsExtract = false
            });

            return _dbContext.SaveChanges() > SystemConstant.Zero;
        }

        /// <summary>
        /// 提取积分
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool ExtractPoints(string userName, int points)
        {
            if (string.IsNullOrWhiteSpace(userName) || points <= 0)
                return false;

            var userEntity = _dbContext.UserKeyEntities.FirstOrDefault(t => t.Userkeylastdate != null && t.Userid == userName);

            if (userEntity == null)
                return false;

            var userPoints = _dbContext.UserPointsEntities.FirstOrDefault(t => t.Userid == userName);

            if (userPoints == null)
                return false;

            if (userPoints.Userpoints < points)
                return false;

            userPoints.Userpoints -= points;
            userPoints.UpdateTime = DateTime.Now;
            _dbContext.UserPointsEntities.Update(userPoints);
            _dbContext.UserPointsRecordEntities.Add(new Domain.Entities.UserPointsRecordEntity
            {
                Userid = userName,
                Userpoints = points,
                Userdata = DateTime.Now,
                IsExtract = true
            });
            return _dbContext.SaveChanges() > SystemConstant.Zero;
        }

        /// <summary>
        /// 查询提取记录
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<PointsDto> GetExtractPoints(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return new List<PointsDto>();

            var userEntity = _dbContext.UserKeyEntities.FirstOrDefault(t => t.Userkeylastdate != null && t.Userid == userName);

            if (userEntity == null)
                return new List<PointsDto>();

            var userPointsRecords = _dbContext.UserPointsRecordEntities.Where(t => t.Userid == userName).OrderByDescending(t => t.Userdata).ToList();

            return userPointsRecords.Select(x => new PointsDto
            {
                Userpoints = x.Userpoints,
                UserPointsDate = x.Userdata.ToString("yyyy-MM-dd HH:mm:ss"),
                IsExtract = x.IsExtract
            }).ToList();
        }


        /// <summary>
        /// 根据账号获取积分
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public PointsDto GetPoints(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName) )
                return new PointsDto();

            var userEntity = _dbContext.UserKeyEntities.FirstOrDefault(t =>  t.Userid == userName);

            if (userEntity == null)
                return new PointsDto();

            var userPoints = _dbContext.UserPointsEntities.FirstOrDefault(t => t.Userid == userName);

            return new PointsDto { UserName=userPoints.Userid, Userpoints = userPoints.Userpoints, UserPointsDate= userPoints.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss") };
        }

        /// <summary>
        /// 获取所有账号积分
        /// </summary>
        /// <returns></returns>
        public List<PointsDto> GetPointsUser()
        {
            var result = new List<PointsDto>();

            var userPoints = _dbContext.UserPointsEntities.ToList();

            foreach (var point in userPoints)
            {
                result.Add(new PointsDto
                {
                    UserName = point.Userid,
                    Userpoints = point.Userpoints,
                    UserPointsDate = point.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")

                });
            }

            return result;

        }

        /// <summary>
        /// 获取所有账号积分记录
        /// </summary>
        /// <returns></returns>
        public List<PointsDto> GetRecordPoints()
        {
            var result = new List<PointsDto>();

            var userPoints = _dbContext.UserPointsRecordEntities.ToList();

            foreach (var point in userPoints)
            {
                result.Add(new PointsDto
                {
                    UserName = point.Userid,
                    Userpoints = point.Userpoints,
                    UserPointsDate = point.Userdata.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsExtract=point.IsExtract

                });
            }

            return result;

        }
    }
}
