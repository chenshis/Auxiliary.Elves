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
        /// 删除卡用户
        /// </summary>
        public const string DelUserRoute = "v1/auxiliary/account/deleteuser";   
        /// <summary>
        /// mac获取卡用户
        /// </summary>
        public const string UserMacRoute = "v1/auxiliary/account/getusermac";

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

        public const string ServerUrl = "http://47.238.158.162/";

        public const string Unauthorized = "权限认证失败";

        public const string RefreshTokenRoute = "";
    }
}
