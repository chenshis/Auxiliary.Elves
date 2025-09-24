using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Infrastructure.Config
{
    /// <summary>
    /// 客户端帮助类
    /// </summary>
    public static class ClientHelper
    {
        /// <summary>
        /// 获取mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMac(this ILogger logger)
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                // 获取网络接口的物理地址（MAC地址）
                PhysicalAddress macAddress = networkInterface.GetPhysicalAddress();

                if (macAddress != null)
                {
                    byte[] bytes = macAddress.GetAddressBytes();
                    string macAddressString = string.Join(":", bytes.Select(b => b.ToString("X2")));
                    logger.LogInformation($"print internet interface：{networkInterface.Name}");
                    return macAddressString;
                }
            }
            logger.LogError($"No network interface obtained");
            return null;
        }
    }
}
