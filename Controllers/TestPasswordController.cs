using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace NetFavorite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestPasswordController : ControllerBase
    {
        // 创建随机 salt
        [HttpGet("CreateSalt")]
        public string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        // 加密
        [HttpGet("HashPassword")]
        public string HashPassword(string password, string salt)
        {
            DateTime t1 = DateTime.Now;
            var valueBytes = KeyDerivation.Pbkdf2(
                password: password, // 密码
                salt: Encoding.UTF8.GetBytes(salt), // 盐
                prf: KeyDerivationPrf.HMACSHA512, // 伪随机函数，这里是 SHA-512
                iterationCount: 600000, // 迭代次数（OWASP 推荐 PBKDF2-HMAC-SHA512 最少 600,000 次）
                numBytesRequested: 256 / 8); // 最后输出的秘钥长度
            string passwordStore = Convert.ToBase64String(valueBytes);
            DateTime t2 = DateTime.Now;
            return $"密码：{passwordStore}，加密用时:{(t2 - t1).TotalMilliseconds.ToString()}毫秒";
        }

        // 传统 Hash 加密 - MD5
        [HttpGet("MD5Encrypt64")]
        public string MD5Encrypt64(string password)
        {
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(s);
        }

        // 传统 Hash 加密 - SHA-1
        [HttpGet("SHA1Encrypt64")]
        public string SHA1Encrypt64(string password)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] s = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(s);
        }

        // 传统 Hash 加密 - SHA-256
        [HttpGet("SHA256Encrypt64")]
        public string SHA256Encrypt64(string password)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] s = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(s);
        }

        // Hash+Salt 方案 - MD5 + Salt
        [HttpGet("MD5WithSalt")]
        public string MD5WithSalt(string password, string salt)
        {
            string passwordWithSalt = password + salt;
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
            return Convert.ToBase64String(s);
        }

        // Hash+Salt 方案 - SHA-256 + Salt
        [HttpGet("SHA256WithSalt")]
        public string SHA256WithSalt(string password, string salt)
        {
            string passwordWithSalt = password + salt;
            SHA256 sha256 = SHA256.Create();
            byte[] s = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
            return Convert.ToBase64String(s);
        }
    }
}
