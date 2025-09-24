using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Infrastructure.Config
{
    public static class CardGenerator
    {
        private static readonly Random random = new Random();
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LettersAndDigits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// 生成单个卡号（8位：大写字母+数字）
        /// </summary>
        public static string GenerateCardNumber(int length = 8)
        {
            return GenerateRandomString(LettersAndDigits, length);
        }

        /// <summary>
        /// 生成单个密码（6位：大写字母）
        /// </summary>
        public static string GeneratePassword(int length = 6)
        {
            return GenerateRandomString(Letters, length);
        }

        /// <summary>
        /// 生成 N 组卡号和密码
        /// </summary>
        public static List<(string CardNumber, string Password)> GenerateCardBatch(int count)
        {
            var result = new List<(string, string)>();
            for (int i = 0; i < count; i++)
            {
                string cardNumber = GenerateCardNumber();
                string password = GeneratePassword();
                result.Add((cardNumber, password));
            }
            return result;
        }

        /// <summary>
        /// 内部通用随机生成方法
        /// </summary>
        private static string GenerateRandomString(string chars, int length)
        {
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
    }
}
