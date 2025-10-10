namespace Auxiliary.Elves.Api.Dtos
{
    public class AccountUserDto
    {
        /// <summary>
        /// 账号唯一编码,;
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 特征码
        /// </summary>
        public string Userfeaturecode { get; set; }

        /// <summary>
        /// 备用数字
        /// </summary>
        public string Userbakckupnumber { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 有效卡数
        /// </summary>
        public int UserNumber { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

        ///// <summary>
        ///// 绑定谷歌账号
        ///// </summary>
        //public string UserBindAccount { get; set; }

        /// <summary>
        /// 绑定地址
        /// </summary>
        public string UserAddress { get; set; }

        /// <summary>
        /// 邀请人
        /// </summary>
        public string UserInviteUserName { get; set; }  
    }
}
