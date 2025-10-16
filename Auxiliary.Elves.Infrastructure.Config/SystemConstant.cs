using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Infrastructure.Config
{
    /// <summary>
    /// 系统常量
    /// </summary>
    public static class SystemConstant
    {
        public const int MaxDay = 30;
        public const int MaxHour = 24;
        public const int MaxPoints = 1;


        public const int Zero = 0;

        public const string ExpireDateStatus = "已到期";

        /// <summary>
        /// 上传视频
        /// </summary>
        public const string VideoUploadRoute = "v1/auxiliary/video/videoupload";

        /// <summary>
        /// 下载视频
        /// </summary>
        public const string VideoDownloadRoute = "v1/auxiliary/video/videodownload";

        /// <summary>
        /// 获取视频地址
        /// </summary>
        public const string VideoVideoUrlRoute = "v1/auxiliary/video/getvideourl";

        /// <summary>
        /// 查询参数
        /// </summary>
        public const string SystemSettingRoute = "v1/auxiliary/systemsettings/querysystemsetting";  
        /// <summary>
        /// 设置参数
        /// </summary>
        public const string SetSystemSettingRoute= "v1/auxiliary/systemsettings/setsystemsetting";
        /// <summary>
        /// 查询公告
        /// </summary>
        public const string AnnouncementRoute = "v1/auxiliary/announcement/queryannouncement";  
        /// <summary>
        /// 删除公告
        /// </summary>
        public const string DelAnnouncementRoute = "v1/auxiliary/announcement/delannouncement";
        /// <summary>
        /// 新增公告
        /// </summary>
        public const string AddAnnouncementRoute = "v1/auxiliary/announcement/addannouncement"; 

        /// <summary>
        /// 查询提取记录
        /// </summary>
        public const string UserPointsRecord = "v1/auxiliary/points/userpointsrecord";
        /// <summary>
        /// 提取积分
        /// </summary>
        public const string ExtractRoute= "v1/auxiliary/points/extract";   
        /// <summary>
        /// 获取积分
        /// </summary>
        public const string PointsRoute= "v1/auxiliary/points/querypoints";

        /// <summary>
        /// 获取所有账号积分
        /// </summary>
        public const string PointsUserRoute = "v1/auxiliary/points/queryuserpoints";

        /// <summary>
        /// 新增积分
        /// </summary>
        public const string AddPointsRoute = "v1/auxiliary/points/addpoints";


        /// <summary>
        /// 新增自定义积分
        /// </summary>
        public const string AddCustomPointsRoute = "v1/auxiliary/points/addcustompoints";

        /// <summary>
        /// 删除卡用户
        /// </summary>
        public const string DelUserRoute = "v1/auxiliary/account/deleteuser";

        /// <summary>
        /// 修改卡用户运行状态
        /// </summary>
        public const string UpdateUserKeyRunRoute = "v1/auxiliary/account/updateuserkeyrun";

        /// <summary>
        /// mac获取卡用户
        /// </summary>
        public const string UserMacRoute = "v1/auxiliary/account/getusermac";
        /// <summary>
        /// 账号获取下面卡密
        /// </summary>
        public const string UserToKeyRoute = "v1/auxiliary/account/getuserkey";
        /// <summary>
        /// 登录路由
        /// </summary>
        public const string LoginRoute = "v1/auxiliary/account/login";

        /// <summary>
        /// 注册路由
        /// </summary>
        public const string RegisterRoute = "v1/auxiliary/account/register";

        /// <summary>
        /// 绑定谷歌
        /// </summary>
        public const string BindGoogleRoute = "v1/auxiliary/account/binggoogle";

        /// <summary>
        /// 绑定地址
        /// </summary>
        public const string BindAddressRoute = "v1/auxiliary/account/bingaddress";

        /// <summary>
        /// 设置用户账号状态
        /// </summary>
        public const string SetEnableStatusRoute = "v1/auxiliary/account/setenablestatus";

        /// <summary>
        /// 查询所有被邀请人
        /// </summary>
        public const string UserInviteInfoRoute= "v1/auxiliary/account/queryuserinviteuser";

        /// <summary>
        /// 修改邀请人
        /// </summary>
        public const string SetUserInviteRoute="v1/auxiliary/account/setuserinvite";

        /// <summary>
        /// 注册卡密路由
        /// </summary>
        public const string RegisterKeyRoute = "v1/auxiliary/account/registerkey";

        /// <summary>
        /// 用户信息
        /// </summary>
        public const string UserRoute = "v1/auxiliary/account/queryuser";

        /// <summary>
        /// 用户卡密信息
        /// </summary>
        public const string UserKeyRoute = "v1/auxiliary/account/queryuserkey";

        /// <summary>
        /// 找回账号
        /// </summary>
        public const string RecoverRoute = "v1/auxiliary/account/recover";

        /// <summary>
        /// 新增服务器账号
        /// </summary>
        public const string RegisterServerRoute = "v1/auxiliary/account/registerserver";


        /// <summary>
        /// 登录服务器路由
        /// </summary>
        public const string LoginServerRoute = "v1/auxiliary/account/loginserver";

        /// <summary>
        /// 刷新token路由
        /// </summary>
        public const string RefreshTokenRoute = "v1/auxiliary/account/refresh-token";


        /// <summary>
        /// 登录客户端
        /// </summary>
        public const string LoginKeyRoute = "v1/auxiliary/account/loginkey";

        /// <summary>
        /// 默认连接
        /// </summary>
        public const string DefaultConnection = nameof(DefaultConnection);

        /// <summary>
        /// 端口
        /// </summary>
        public const string HostFileName = "host.json";

        /// <summary>
        /// 宿主地址列表
        /// </summary>
        public const string HostPort = "HostPort";

        public const string ServerUrl = "http://autojl110.cc/";
        public const string ServerVideoUrl = "http://autojl110.cc/v2/";

        public const string Unauthorized = "权限认证失败";
        /// <summary>
        /// 用户名长度
        /// </summary>
        public const string ErrorUserNameLengthMessage = "登录名长度不在指定范围内！";
        /// <summary>
        /// 错误用户提示消息
        /// </summary>
        public const string ErrorNotExistUserNameMessage = "用户信息不存在！";
        /// <summary>
        /// 出错密码提示消息
        /// </summary>
        public const string ErrorEmptyPasswordMessage = "密码不能为空！";

        /// <summary>
        /// 错误用户提示消息
        /// </summary>
        public const string ErrorEmptyUserNameMessage = "用户名不能为空！";

        /// <summary>
        /// 错误用户或密码
        /// </summary>
        public const string ErrorUserOrPasswordMessage = "用户名或密码不正确！";
        /// <summary>
        /// 异常空消息
        /// </summary>
        public const string ErrorEmptyMessage = "{0}不能为空！";
        public const string ErrorExistMessage = "{0}已存在！";

     

        /* jwt 系统常量 */
        public const string JwtSecurityKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDI2a2EJ7m872v0afyoSDJT2o1+SitIeJSWtLJU8/Wz2m7gStexajkeD+Lka6DSTy8gt9UwfgVQo6uKjVLG5Ex7PiGOODVqAEghBuS7JzIYU5RvI543nNDAPfnJsas96mSA7L/mD7RTE2drj6hf3oZjJpMPZUQI/B1Qjb5H3K3PNwIDAQAB";
        public const string JwtAudience = "http://localhost:5245";
        public const string JwtIssuer = "http://localhost:5245";
        public const string JwtActor = "This is a stock trading function, welcome to use!";

        /// <summary>
        /// 刷新token异常
        /// </summary>
        public const string ErrorRefreshTokenFailMessage = "token刷新失败，请重新登录！";
    }
}
