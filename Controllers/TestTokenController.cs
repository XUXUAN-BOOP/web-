using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetFavorite.Models;
using NetFavorite.Utilities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NetFavorite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestTokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public TestTokenController(IConfiguration configuration, ITokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }

        // 第二题：生成Token（使用配置中的密钥）
        [HttpGet("GetToken")]
        [Authorize]
        public string GetToken()
        {
            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JWT:SecretKey"] ?? "www.czie.edu.cn123456789www.czie.edu.cn123456789"));
            var credential = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new System.Security.Claims.Claim("UserID", "1001"),
                new System.Security.Claims.Claim("UserName", "谢帅")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credential
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // 使用 TokenService 生成 Token（新增测试方法）
        [HttpGet("GetToken2")]
        [Authorize]
        public string GetToken2()
        {
            return _tokenService.CreateToken(Guid.NewGuid(), "admin", "系统管理员");
        }

        // 第四题：读取Token数据（原有方法保留）
        [HttpGet("ReadToken")]
        [Authorize]
        public string ReadToken([FromHeader] string Authorization)
        {
            var token = Authorization.Substring(7);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            JwtPayload payload = jwtToken.Payload;

            string strUserId = payload.ContainsKey("UserID") ? payload["UserID"]?.ToString() ?? "" : "";
            string strUserName = payload.ContainsKey("UserName") ? payload["UserName"]?.ToString() ?? "" : "";

            return $"UserID:{strUserId},UserName:{strUserName}";
        }

        // 使用 TokenService 读取 Token（新增测试方法）
        [HttpGet("ReadToken2")]
        [Authorize]
        public LoginUser ReadToken2()
        {
            return _tokenService.ReadToken();
        }
    }
}
