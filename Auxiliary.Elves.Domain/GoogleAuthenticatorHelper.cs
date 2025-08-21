using Google.Authenticator;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Domain
{
    public static class GoogleAuthenticatorHelper
    {
        public static string GenerateSecretKey(string userId)
        {
            // 当前时间戳（秒）
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // 用用户ID+时间戳生成随机种子
            string input = $"{userId}-{timestamp}";

            using (var hmac = new HMACSHA1())
            {
                byte[] hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                // 取前 20 字节生成 TOTP 密钥
                byte[] key = new byte[20];
                Array.Copy(hash, key, 20);

                string base32Secret = Google.Authenticator.Base32Encoding.ToString(key);
                return base32Secret;
            }
        }

        /// <summary>
        /// 验证用户输入的 Google Authenticator 动态码是否有效
        /// </summary>
        /// <param name="secretKey">用户绑定的 Base32 秘钥</param>
        /// <param name="userInput">用户输入的 6 位验证码</param>
        /// <returns>是否有效</returns>
        public static bool VerifyTotpCode(string secretKey, string userInput)
        {
            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(userInput))
                return false;

            try
            {
                var totp = new Totp(Google.Authenticator.Base32Encoding.ToBytes(secretKey));
                // VerificationWindow(1,1) 允许前后各一个 30 秒误差
                return totp.VerifyTotp(userInput, out long timeStepMatched, new VerificationWindow(1, 1));
            }
            catch
            {
                return false;
            }
        }
    }
}
