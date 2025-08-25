using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain;
using Auxiliary.Elves.Domain.Entities;
using Auxiliary.Elves.Infrastructure.Config;

namespace Auxiliary.Elves.Api.ApiService
{
    public class LoginApiService : ILoginApiService
    {
        private readonly AuxiliaryDbContext _dbContext;

        public LoginApiService(AuxiliaryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<UserDto> GetAllUser(string userName, bool enabled)
        {
            var userDtos = new List<UserDto>(); 

            if (string.IsNullOrWhiteSpace(userName))
                return userDtos;

            var userEntities = _dbContext.UserKeyEntities.Where(t => 
              enabled ? t.Userkeylastdate != null : t.Userkeylastdate == null && t.Userid == userName).ToList();

            if(!userEntities.Any())
                return userDtos;

            userDtos.AddRange(
                userEntities.Select(x => new UserDto
                {
                    Userkey = x.Userkey,
                    Userkeyid = x.Userkeyid,
                    Userkeylastdate = x.Userkeylastdate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    Userid = x.Userid,
                    Isonline = x.Isonline,
                })); 

            return userDtos;

        }

        public bool Login(AccountRequestDto accountRequest)
        {
            if (string.IsNullOrWhiteSpace(accountRequest.UserName) || string.IsNullOrWhiteSpace(accountRequest.UserKeyId) ||
                string.IsNullOrWhiteSpace(accountRequest.Password) || string.IsNullOrWhiteSpace(accountRequest.Mac))
                return false;

            var userEntity = _dbContext.UserKeyEntities.FirstOrDefault(t => t.Userid == accountRequest.UserName
                                                                        && t.Userkeyid == accountRequest.UserKeyId
                                                                        && t.Userkey == accountRequest.Password);

            if (userEntity == null)
                return false;

            if (!string.IsNullOrWhiteSpace(userEntity.Userkeyip))
            {
                if ((DateTime.Now - userEntity.Userkeylastdate.Value).Days > SystemConstant.MaxDay)
                    return false;

                if (!userEntity.Isonline && (DateTime.Now - userEntity.UpdateTime).Hours < SystemConstant.MaxHour)
                    return false;

                if (userEntity.Userkeyip != accountRequest.Mac)
                    userEntity.Isonline = false;
            }
            else
            {
                userEntity.Userkeylastdate = DateTime.Now;
                userEntity.Isonline = true;
            }

            userEntity.Userkeyip = accountRequest.Mac;
            _dbContext.UserKeyEntities.Update(userEntity);

            var result = _dbContext.SaveChanges();

            if (result < SystemConstant.Zero)
                return false;

            return userEntity.Isonline;
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
                userName = _rnd.Next(100000000, 100000000).ToString(); // 8位数
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

            var userEntity = _dbContext.UserEntities.FirstOrDefault(x => x.Userid == userId);

            if (userEntity == null)
                return false;

            Random _rnd = new Random();
            HashSet<string> _used = new HashSet<string>();

            Random _rndKeyId = new Random();
            HashSet<string> _usedKey = new HashSet<string>();

            string userKey;
            string userKeyId;

            foreach (var item in _dbContext.UserKeyEntities.ToList())
            {
                _used.Add(item.Userkey);
                _usedKey.Add(item.Userkeyid);
            }


            do
            {
                userKeyId = CardGenerator.GenerateCardNumber(8);
            } while (!_usedKey.Add(userKeyId));

            do
            {
                userKey = CardGenerator.GenerateCardNumber(6);
            } while (!_used.Add(userKey));

            UserKeyEntity keyEntity = new UserKeyEntity();
            keyEntity.Userid = userEntity.Username;
            keyEntity.Userkeyid = userKeyId;
            keyEntity.Userkey = userKey;
            keyEntity.Isonline = false;
            _dbContext.UserKeyEntities.Add(keyEntity);
            var result = _dbContext.SaveChanges();
            return result > SystemConstant.Zero;
        }
    }
}
