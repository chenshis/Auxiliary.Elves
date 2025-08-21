using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain;
using Auxiliary.Elves.Domain.Entities;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.ApiService
{
    public class LoginApiService : ILoginApiService
    {
        private readonly AuxiliaryDbContext _dbContext;



        public LoginApiService(AuxiliaryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ApiResponse<string> Login(AccountRequestDto accountRequest)
        {
            throw new NotImplementedException();
        }

        public string Register(string userFeatureCode)
        {
            if (string.IsNullOrWhiteSpace(userFeatureCode))
                return "";

            var userEntity = _dbContext.UserEntities.FirstOrDefault(x => x.Userfeaturecode == userFeatureCode);

            if (userEntity != null)
                return "";

            Random _rnd = new Random();
            HashSet<string> _used = new HashSet<string>();

            Random _rndNumber = new Random();
            HashSet<string> _usedNumber = new HashSet<string>();

            string userName;
            string userNumber;

            foreach (var item in _dbContext.UserEntities.ToList())
            {
                _used.Add(item.Userfeaturecode);
                _usedNumber.Add(item.Userbakckupnumber);
            }

            do
            {
                userName = _rnd.Next(1000000, 10000000).ToString(); // 7位数
            } while (!_used.Add(userName)); // HashSet 保证唯一

            do
            {
                userNumber = _rndNumber.Next(10000000, 100000000).ToString(); // 8位数
            } while (!_usedNumber.Add(userNumber)); // HashSet 保证唯一

            UserEntity user = new UserEntity();
            user.Userid = GoogleAuthenticatorHelper.GenerateSecretKey(userName);
            user.Userfeaturecode = userFeatureCode;
            user.Username = userName;
            user.Userbakckupnumber = userNumber;
            _dbContext.UserEntities.Add(user);

            var result = _dbContext.SaveChanges();
            return result > SystemConstant.Zero ? user.Userid : "";
        }

        public bool RegisterKey(string userId, string verCode)
        {
            var isAuthenticator = GoogleAuthenticatorHelper.VerifyTotpCode(userId, verCode);

            if (!isAuthenticator)
                return false;

            return true;
        }
    }
}
