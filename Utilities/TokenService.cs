using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NetFavorite.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetFavorite.Utilities
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        private SymmetricSecurityKey GetSecretKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? _configuration["JWT:SecretKey"]
                ?? throw new InvalidOperationException("JWT 密钥未配置，请设置 JWT_SECRET_KEY 环境变量或配置 JWT:SecretKey")
            ));
        }

        public string CreateToken(Guid userId, string userName, string userRole)
        {
            // 1. 创建 Claims
            var claims = new[]
            {
                new Claim("UserID", userId.ToString()),
                new Claim("UserName", userName),
                new Claim("UserRole", userRole),
                new Claim(ClaimTypes.Role, userRole)
            };

            // 2. 获取密钥
            var secretKey = GetSecretKey();

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成 Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. 从 appsettings.json 中读取 Expires
            var expires = Convert.ToDouble(_configuration["JWT:Expires"]);

            // 6. 根据以上，生成 token
            var token = new JwtSecurityToken(
                _configuration["JWT:Issuer"],           // 签发人，创建Token的人
                _configuration["JWT:Audience"],         // 受众，使用Token的人
                claims,                                 // 具体数据
                DateTime.Now,                           // Not Before：生效时间，在此以前是无效的
                DateTime.Now.AddMinutes(expires),       // 过期时间
                signingCredentials                      // 签证信息，密钥 + 加密方式
            );

            // 7. 将 token 变为 string
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }

        public LoginUser ReadToken()
        {
            var emptyUser = new LoginUser
            {
                LoginUser_Id = Guid.Empty,
                LoginUser_Account = "",
                LoginUser_Role = ""
            };

            string? authHeader = _contextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return emptyUser;
            }

            string token = authHeader.Substring(7); // 去除"Bearer+空格"的开头部分

            try
            {
                // 使用 ValidateToken 验证签名和有效性
                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = GetSecretKey(),
                    ValidateIssuerSigningKey = true
                };

                handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // 从验证通过的 token 中读取 Claims
                var jwtToken = (JwtSecurityToken)validatedToken;
                var payload = jwtToken.Payload;

                Guid userId = Guid.Empty;
                string? userName = "";
                string? userRole = "";

                if (payload.ContainsKey("UserID"))
                {
                    string? strUserId = payload["UserID"]?.ToString();
                    if (!string.IsNullOrEmpty(strUserId))
                    {
                        userId = new Guid(strUserId);
                    }
                }

                if (payload.ContainsKey("UserName"))
                {
                    userName = payload["UserName"].ToString();
                }

                if (payload.ContainsKey("UserRole"))
                {
                    userRole = payload["UserRole"].ToString();
                }

                if (userId == Guid.Empty || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userRole))
                {
                    return emptyUser;
                }

                return new LoginUser
                {
                    LoginUser_Id = userId,
                    LoginUser_Account = userName,
                    LoginUser_Role = userRole
                };
            }
            catch (Exception ex)
            {
                // Token 验证失败（签名无效、过期等）
                _logger.LogWarning(ex, "Token 验证失败");
                return emptyUser;
            }
        }
    }
}