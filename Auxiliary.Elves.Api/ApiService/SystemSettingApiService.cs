using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain;
using Auxiliary.Elves.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.ApiService
{
    public class SystemSettingApiService : ISystemSettingApiService
    {
        private readonly AuxiliaryDbContext _dbContext;

        public SystemSettingApiService(AuxiliaryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SystemSettingRequestDto GetSystemSetting()
        {
            var systemSettingEntity= _dbContext.SystemSettingEntity.FirstOrDefault();

            if (systemSettingEntity == null)
                return new SystemSettingRequestDto();

            return new SystemSettingRequestDto
            {
                Handlingfee = systemSettingEntity.Handlingfee,
                Maxinterval = systemSettingEntity.Maxinterval,
                Minnumber = systemSettingEntity.Minnumber,
                Mininterval = systemSettingEntity.Mininterval
            };
        }

        public bool SetSystemSetting(SystemSettingRequestDto settingRequestDto)
        {
            var systemSettingEntity = _dbContext.SystemSettingEntity.FirstOrDefault();

            if (systemSettingEntity == null)
            {
                _dbContext.SystemSettingEntity.Add(new Domain.Entities.SystemSettingEntity
                {
                    Handlingfee = settingRequestDto.Handlingfee,
                    Maxinterval = settingRequestDto.Maxinterval,
                    Minnumber = settingRequestDto.Minnumber,
                    Mininterval = settingRequestDto.Mininterval
                });

            }
            else
            {
                systemSettingEntity.Handlingfee = settingRequestDto.Handlingfee;
                systemSettingEntity.Maxinterval = settingRequestDto.Handlingfee;
                systemSettingEntity.Minnumber = settingRequestDto.Handlingfee;
                systemSettingEntity.Mininterval = settingRequestDto.Handlingfee;

                _dbContext.SystemSettingEntity.Update(systemSettingEntity);
            }
            return _dbContext.SaveChanges() > SystemConstant.Zero ;
        }
    }
}
