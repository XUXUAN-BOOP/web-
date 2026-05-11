using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace NetFavorite.Utilities
{
    public class HashPasswordService
    {
        //创建随机 Salt
        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        //加密
        public static string HashPassword(string password, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: password, // 密码
                salt: Encoding.UTF8.GetBytes(salt), // 盐
                prf: KeyDerivationPrf.HMACSHA512, // 伪随机函数，这里是 SHA-512
                iterationCount: 600000, // 迭代次数（OWASP 推荐 PBKDF2-HMAC-SHA512 最少 600,000 次）
                numBytesRequested: 256 / 8); // 最后输出的秘钥长度
            return Convert.ToBase64String(valueBytes);
        }

        //验证
        public static bool Validate(string password, string salt, string passwordStore)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (string.IsNullOrEmpty(salt)) return false;
            if (string.IsNullOrEmpty(passwordStore)) return false;

            return HashPassword(password, salt) == passwordStore;
        }
    }
}
