using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain;
using Auxiliary.Elves.Domain.Entities;
using Auxiliary.Elves.Infrastructure.Config;
using Auxiliary.Elves.Infrastructure.Config.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Auxiliary.Elves.Api.ApiService
{
    public class LoginApiService : ILoginApiService
    {
        private readonly AuxiliaryDbContext _dbContext;

        public LoginApiService(AuxiliaryDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        /// <summary>
        /// 登陆服务器
        /// </summary>
        /// <param name="accountRequest"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public UserServerEntity LoginServer(UserRequestDto accountRequest)
        {
            if (string.IsNullOrWhiteSpace(accountRequest.Password))
            {
                throw new Exception(SystemConstant.ErrorEmptyPasswordMessage);
            }
            if (string.IsNullOrWhiteSpace(accountRequest.UserName))
            {
                throw new Exception(SystemConstant.ErrorEmptyUserNameMessage);
            }
            var userPassword = accountRequest.Password.GetMd5();
            var userName = accountRequest.UserName;
            var userEntity = _dbContext.UserServerEntities.FirstOrDefault(t => t.UserName == userName && t.Password == userPassword);
            if (userEntity == null)
            {
                throw new Exception(SystemConstant.ErrorUserOrPasswordMessage);
            }
            return userEntity;
        }

        public bool AddUser(UserRequestDto request)
        {
            RegisterValidate(request);

            string GetErrorMessage(string propertyName, string errorTemplate)
            {
                var errorName = GetDocSummary(typeof(UserRequestDto), propertyName);
                return string.Concat("", string.Format(errorTemplate, errorName));
            }
            var userEntity = _dbContext.UserServerEntities.FirstOrDefault(t => t.UserName == request.UserName);

            if (userEntity != null)
            {
                if (userEntity.UserName == request.UserName)
                {
                    throw new Exception(GetErrorMessage(nameof(request.UserName), SystemConstant.ErrorExistMessage));
                }
            }
            var addUserEntity = new UserServerEntity();
            addUserEntity.UserName = request.UserName;
            addUserEntity.Password = request.Password.GetMd5();
            addUserEntity.Role = RoleEnum.Admin;
            addUserEntity.Expires =DateTime.Now;
            addUserEntity.Status = true;
            _dbContext.UserServerEntities.Add(addUserEntity);
            var result = _dbContext.SaveChanges();

            return result > SystemConstant.Zero;
        }

        /// <summary>
        /// 根据账号查询所有被邀请用户
        /// </summary>
        /// <param name="userName">账号</param>
        /// <returns></returns>
        public List<AccountUserDto> GetUserInviteUserInfo(string userName)
        {
            var userDtos = new List<AccountUserDto>();

            if (string.IsNullOrEmpty(userName)) { return  userDtos; }

            var userEntity = _dbContext.UserEntities.ToList();

            var userInfo= userEntity.FirstOrDefault(x => x.UserName == userName);

            if (userInfo == null) { return userDtos; }

            var userList=GetAllChildren(userEntity,userInfo.UserName);

            foreach (var user in userList)
            {
                userDtos.Add(new AccountUserDto
                {
                    Userid = user.UserId,
                    UserName = user.UserName,
                    Userfeaturecode = user.UserFeatureCode,
                    Userbakckupnumber = user.UserBakckupNumber,
                    CreateTime = user.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsEnable = user.IsEnable
                });
            }


            return userDtos;
        }

        public List<UserEntity> GetAllChildren(List<UserEntity> allUsers, string parentUserName)
        {
            var result = new List<UserEntity>();

            // 找出直接下级
            var children = allUsers.Where(x => x.UserInviteUserName == parentUserName).ToList();
            foreach (var child in children)
            {
                result.Add(child);
                // 递归查找 child 的下级
                result.AddRange(GetAllChildren(allUsers, child.UserName));
            }

            return result;
        }

        /// <summary>
        /// 根据账号修改被邀请人
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userInviteUserName">邀请人</param>
        /// <returns></returns>
        public bool SetUserInvite(string userName, string userInviteUserName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            var userEntity = _dbContext.UserEntities.FirstOrDefault(t => t.UserName == userName);

            if (userEntity == null)
                return false;

            userEntity.UserInviteUserName = userInviteUserName;

            _dbContext.UserEntities.Update(userEntity);

            return _dbContext.SaveChanges() > SystemConstant.Zero;
        }

        public bool DeleteUser(string userkeyid)
        {
            if (string.IsNullOrWhiteSpace(userkeyid))
                return false;

            var userEntity = _dbContext.UserKeyEntities.FirstOrDefault(t =>  t.Userkeyid == userkeyid);

            if (userEntity == null)
                return false;

            userEntity.Isonline = false;

            _dbContext.UserKeyEntities.Update(userEntity);

            return _dbContext.SaveChanges() > SystemConstant.Zero;
        }

        public bool UpdateUserKeyRun(string userkeyid, bool isRun)
        {
            if (string.IsNullOrWhiteSpace(userkeyid))
                return false;

            var userEntity = _dbContext.UserKeyEntities.FirstOrDefault(t => t.Userkeyid == userkeyid);

            if (userEntity == null)
                return false;

            userEntity.IsRun = isRun;

            _dbContext.UserKeyEntities.Update(userEntity);

            return _dbContext.SaveChanges() > SystemConstant.Zero;
        }

        public List<UserDto> GetAllUserKey(string userFeatureCode)
        {
            var userDtos = new List<UserDto>();

            var userName = "";

            if (!string.IsNullOrWhiteSpace(userFeatureCode))
            {
                var userInfo = _dbContext.UserEntities.FirstOrDefault(x => x.UserFeatureCode == userFeatureCode);

                if (userInfo != null)
                    userName = userInfo.UserName;
            }

            var userEntities = !string.IsNullOrWhiteSpace(userName) ?
                _dbContext.UserKeyEntities.Where(x => x.Userid == userName).ToList()
                : _dbContext.UserKeyEntities.ToList();
            
            if (!userEntities.Any())
                return userDtos;


            foreach (var user in userEntities)
            {
                var userInfo = new UserDto
                {
                    Userkey = user.Userkey,
                    Userkeyid = user.Userkeyid,
                    Userkeylastdate = user.Userkeylastdate!=null? user.Userkeylastdate.Value.ToString("yyyy-MM-dd HH:mm:ss"):"",
                    Userid = user.Userid,
                    IsRun=user.IsRun,
                    Isonline = user.Isonline
                };

                if (!string.IsNullOrWhiteSpace(user.Userkeyip)&& user.Userkeylastdate != null)
                {
                    DateTime expireDate = user.Userkeylastdate.Value.AddDays(SystemConstant.MaxDay);

                    if (expireDate > DateTime.Now)
                    {
                        userInfo.ExpireDate = expireDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        userInfo.ExpireDate = SystemConstant.ExpireDateStatus;
                    }
                    
                }

                userDtos.Add(userInfo);
            }

            return userDtos;

        }

        public List<UserDto> GetMacAllUser(string mac)
        {
            var userDtos = new List<UserDto>();

            if (string.IsNullOrWhiteSpace(mac))
                return userDtos;

            var userEntities = _dbContext.UserKeyEntities.Where(t => t.Userkeylastdate != null
           && t.Isonline && t.Userkeyip == mac).ToList();

            if (!userEntities.Any())
                return userDtos;

            foreach (var user in userEntities)
            {
                var userInfo = new UserDto
                {
                    Userkey = user.Userkey,
                    Userkeyid = user.Userkeyid,
                    Userkeylastdate = user.Userkeylastdate != null ? user.Userkeylastdate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                    Userid = user.Userid,
                    IsRun=user.IsRun,
                    Isonline = user.Isonline,
                };

                if (!string.IsNullOrWhiteSpace(user.Userkeyip) && user.Userkeylastdate != null)
                {
                    DateTime expireDate = user.Userkeylastdate.Value.AddDays(SystemConstant.MaxDay);

                    if (expireDate > DateTime.Now)
                    {
                        userInfo.ExpireDate = expireDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        userInfo.ExpireDate = SystemConstant.ExpireDateStatus;
                    }
                }
                userDtos.Add(userInfo);
            }

          
            return userDtos;
        }

        /// <summary>
        /// 根据账号查询下面所有卡密
        /// </summary>
        /// <param name="userId">账号</param>
        /// <returns></returns>
        public List<UserDto> GetUserAllKey(string userId)
        {
            var userDtos = new List<UserDto>();

            if (string.IsNullOrWhiteSpace(userId))
                return userDtos;

            var userEntities = _dbContext.UserKeyEntities.Where(t =>  t.Userid == userId).ToList();

            if (!userEntities.Any())
                return userDtos;

            foreach (var user in userEntities)
            {
                var userInfo = new UserDto
                {
                    Userkey = user.Userkey,
                    Userkeyid = user.Userkeyid,
                    Userkeylastdate = user.Userkeylastdate != null ? user.Userkeylastdate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                    Userid = user.Userid,
                    IsRun=user.IsRun,
                    Isonline = user.Isonline,
                };

                if (!string.IsNullOrWhiteSpace(user.Userkeyip) && user.Userkeylastdate != null)
                {
                    DateTime expireDate = user.Userkeylastdate.Value.AddDays(SystemConstant.MaxDay);

                    if (expireDate > DateTime.Now)
                    {
                        userInfo.ExpireDate = expireDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        userInfo.ExpireDate = SystemConstant.ExpireDateStatus;
                    }

                }
                userDtos.Add(userInfo);
            }


            return userDtos;
        }

        public List<AccountUserDto> GetAllUser(string userFeatureCode)
        {
            var userDtos = new List<AccountUserDto>();

            var userEntity = _dbContext.UserEntities.ToList();

            var userEntityKey = _dbContext.UserKeyEntities.ToList();

            var userPoints = _dbContext.UserPointsEntities.ToList();

            if (!string.IsNullOrWhiteSpace(userFeatureCode))
                userEntity = userEntity.Where(x => x.UserFeatureCode == userFeatureCode).ToList();

            foreach (var user in userEntity)
            {
                var userKeyCount = userEntityKey.Count(t => t.Userid == user.UserName);

                var points = userPoints.FirstOrDefault(x => x.Userid == user.UserName);

                userDtos.Add(new AccountUserDto
                {
                    Userid = user.UserId,
                    UserName = user.UserName,
                    Userfeaturecode = user.UserFeatureCode,
                    Userbakckupnumber = user.UserBakckupNumber,
                    CreateTime = user.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsEnable = user.IsEnable,
                    UserAddress = user.UserAddress,
                    UserInviteUserName = user.UserInviteUserName,
                    Userpoints= points!=null ? points.Userpoints : 0,
                    UserNumber = userKeyCount
                });
            }

            return userDtos.OrderByDescending(x=>x.CreateTime).ToList();
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
                if ((DateTime.Now - userEntity.CreateTime).Days > SystemConstant.MaxDay)
                    return false;

                if (userEntity.IsLock && (DateTime.Now - userEntity.Userkeylastdate.Value).Hours < SystemConstant.MaxHour)
                    return false;

                if (userEntity.Userkeyip != accountRequest.Mac)
                {
                    userEntity.Isonline = false;
                    userEntity.IsLock = true;
                }
                else
                {
                    userEntity.Userkeylastdate = DateTime.Now;
                    userEntity.Isonline = true;
                }
            }
            else
            {
                userEntity.Userkeylastdate = DateTime.Now;
                userEntity.Isonline = true;
            }

            if (userEntity.Isonline)
                userEntity.IsRun = true;
            
            userEntity.Userkeyip = accountRequest.Mac;

            _dbContext.UserKeyEntities.Update(userEntity);

            var result = _dbContext.SaveChanges();

            if (result < SystemConstant.Zero)
                return false;

            return userEntity.Isonline;
        }

        public string RecoverAccount(string userId, string verCode)
        {
            var isAuthenticator = GoogleAuthenticatorHelper.VerifyTotpCode(userId, verCode);

            if (!isAuthenticator)
                return "";

            var userEntity = _dbContext.UserEntities.FirstOrDefault(x => x.UserId == userId);

            if (userEntity == null)
                return "";

            return userEntity.UserName;
        }

        public bool Register(string userFeatureCode,string userInviteUserName)
        {
            if (string.IsNullOrWhiteSpace(userFeatureCode))
                return false;

            var userEntity = _dbContext.UserEntities.FirstOrDefault(x => x.UserFeatureCode == userFeatureCode);

            if (userEntity != null)
                return false;

            Random _rnd = new Random();
            HashSet<string> _used = new HashSet<string>();

            Random _rndNumber = new Random();
            HashSet<string> _usedNumber = new HashSet<string>();

            string userName;
            string userNumber;

            foreach (var item in _dbContext.UserEntities.ToList())
            {
                _used.Add(item.UserName);
                _usedNumber.Add(item.UserBakckupNumber);
            }

            do
            {
                userName = _rnd.Next(10000000, 100000000).ToString(); // 8位数
            } while (!_used.Add(userName)); // HashSet 保证唯一

            do
            {
                userNumber = _rndNumber.Next(10000000, 100000000).ToString(); // 8位数
            } while (!_usedNumber.Add(userNumber)); // HashSet 保证唯一

            UserEntity user = new UserEntity();
            user.UserId ="";
            user.UserAddress = "";
            user.UserFeatureCode = userFeatureCode;
            user.UserName = userName;
            user.UserBakckupNumber = userNumber;
            user.IsEnable = true;
            user.UserBindAccount = GoogleAuthenticatorHelper.GenerateSecretKey(userName);
            user.UserInviteUserName = userInviteUserName;
            user.CreateTime = DateTime.Now;
            _dbContext.UserEntities.Add(user);

            var result = _dbContext.SaveChanges();
            return result > SystemConstant.Zero;
        }

        /// <summary>
        /// 绑定地址
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userAddress">地址</param>
        /// <returns></returns>
        public bool BindAddress(string userName, string userAddress) 
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userAddress))
                return false;

            var userEntity = _dbContext.UserEntities.FirstOrDefault(x => x.UserName == userName);

            if (userEntity == null || string.IsNullOrWhiteSpace(userAddress))
                return false;

            userEntity.UserAddress = userAddress;

            _dbContext.UserEntities.Update(userEntity);

            var result = _dbContext.SaveChanges();

            return result > SystemConstant.Zero;
        }

        /// <summary>
        /// 绑定谷歌
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userId">谷歌账号</param>
        /// <returns></returns>
        public bool BindGoogle(string userName, string userId) 
        {
            if (string.IsNullOrWhiteSpace(userName)||string.IsNullOrWhiteSpace(userId))
                return false;

            var userEntity = _dbContext.UserEntities.FirstOrDefault(x => x.UserName == userName);

            if (userEntity == null || !string.IsNullOrWhiteSpace(userEntity.UserId))
                return false;

            userEntity.UserId = userId;

            _dbContext.UserEntities.Update(userEntity);

            var result = _dbContext.SaveChanges();

            return result > SystemConstant.Zero;
        }

        /// <summary>
        /// 用户设置是否有效
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userId">谷歌密钥</param>
        /// <returns></returns>
        public bool SetEnableStatus(string userName, bool isEnable)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            var userEntity = _dbContext.UserEntities.FirstOrDefault(x => x.UserName == userName);

            if (userEntity == null)
                return false;

            userEntity.IsEnable = isEnable;

            _dbContext.UserEntities.Update(userEntity);

            var result = _dbContext.SaveChanges();

            return result > SystemConstant.Zero;
        }
      
        public List<UserDto> RegisterKey(string userName, int userNumber)
        {
            //var isAuthenticator = GoogleAuthenticatorHelper.VerifyTotpCode(userId, verCode);

            //if (!isAuthenticator)
            //    return false;

            var resultUser = new List<UserDto>();

            var userEntity = _dbContext.UserEntities.FirstOrDefault(x => x.UserName == userName && x.IsEnable == true );

            if (userEntity == null)
                return resultUser;

            var result = 0;

            for (int i = 0; i < userNumber; i++)
            {
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
                keyEntity.Userid = userEntity.UserName;
                keyEntity.Userkeyid = userKeyId;
                keyEntity.Userkey = userKey;
                keyEntity.Isonline = false;
                keyEntity.IsRun = false;
                keyEntity.IsLock = false;
                keyEntity.CreateTime = DateTime.Now;
                _dbContext.UserKeyEntities.Add(keyEntity);
                result = _dbContext.SaveChanges();

                resultUser.Add(new UserDto
                {
                    Userid = keyEntity.Userid,
                    Userkeyid = keyEntity.Userkeyid,
                    Userkey = keyEntity.Userkey,
                    Userkeylastdate = "",
                    IsRun= keyEntity.IsRun,
                    Isonline = keyEntity.Isonline,
                    ExpireDate = ""
                });
            }
           
            return resultUser;
        }


        private void RegisterValidate(RegisterRequestDto registerRequest)
        {
            string GetErrorMessage(string propertyName, string errorTemplate)
            {
                var errorName = GetDocSummary(typeof(RegisterRequestDto), propertyName);
                return string.Concat("", string.Format(errorTemplate, errorName));
            }
            if (string.IsNullOrWhiteSpace(registerRequest.UserName))
            {
                throw new Exception(GetErrorMessage(nameof(registerRequest.UserName), SystemConstant.ErrorEmptyMessage));
            }
            var userNameLength = registerRequest.UserName.Length;
            if (userNameLength < 5 || userNameLength > 20)
            {
                throw new Exception(SystemConstant.ErrorUserNameLengthMessage);
            }
            if (string.IsNullOrWhiteSpace(registerRequest.Password))
            {
                throw new Exception(GetErrorMessage(nameof(registerRequest.Password), SystemConstant.ErrorEmptyMessage));
            }
          
        }

        /// <summary>
        /// 获取属性名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        private string GetDocSummary(Type type, string propertyName)
        {
            return type.GetProperty(propertyName).GetXmlDocsSummary();
        }

    }
}
