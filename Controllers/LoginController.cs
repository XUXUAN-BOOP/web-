using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetFavorite.Models;
using NetFavorite.Utilities;

namespace NetFavorite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly NetFavoriteDbContext _db;
        private readonly Utilities.ITokenService _tokenService;

        public LoginController(NetFavoriteDbContext db, Utilities.ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        /// <summary>
        /// 用户登录 - POST 方式，密码通过请求正文传递，使用 HashPasswordService 验证
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Models.DataModelLogin>> Post([FromBody] LoginRequest request)
        {
            var user = await _db.LoginUser.FirstOrDefaultAsync(it => it.LoginUser_Account == request.Account);
            if (user == null) return Unauthorized("用户名或密码错误");

            if (Utilities.HashPasswordService.Validate(request.Password, user.LoginUser_Salt!, user.LoginUser_Password))
            {
                string token = _tokenService.CreateToken(user.LoginUser_Id, user.LoginUser_Account, user.LoginUser_Role);
                return new Models.DataModelLogin
                {
                    Id = user.LoginUser_Id,
                    Account = user.LoginUser_Account,
                    Role = user.LoginUser_Role,
                    Token = token
                };
            }
            else return Unauthorized("用户名或密码错误");
        }

        /// <summary>
        /// 修改密码 - PATCH 方式，验证旧密码并加密新密码
        /// </summary>
        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> Patch([FromBody] ChangePasswordRequest request)
        {
            // 新密码不能为空
            if (string.IsNullOrEmpty(request.NewPassword)) return BadRequest("新密码不能为空");

            // 新旧密码不能相同（密码应区分大小写）
            if (string.Equals(request.OldPassword, request.NewPassword, StringComparison.Ordinal)) return BadRequest("新旧密码不能相同");

            // 只能修改自己的密码
            var tokenUser = _tokenService.ReadToken();
            if (tokenUser.LoginUser_Id != request.Id) return Forbid();

            // 判断用户数据是否存在
            var user = _db.LoginUser.FirstOrDefault(it => it.LoginUser_Id == request.Id);
            if (user == null) return NotFound("用户不存在");

            // 验证旧密码
            if (!Utilities.HashPasswordService.Validate(request.OldPassword, user.LoginUser_Salt!, user.LoginUser_Password))
            {
                return Unauthorized("旧密码错误");
            }

            // 修改密码，写入数据库
            // 生成新的随机 salt，再加密密码
            user.LoginUser_Salt = Utilities.HashPasswordService.CreateSalt();
            user.LoginUser_Password = Utilities.HashPasswordService.HashPassword(request.NewPassword, user.LoginUser_Salt);

            _db.Update(user);
            await _db.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 测试 HashPasswordService - 创建 Salt
        /// </summary>
        [HttpGet("test/salt")]
        [Authorize]
        public ActionResult TestCreateSalt()
        {
            string salt = HashPasswordService.CreateSalt();
            return Ok(new { salt = salt });
        }

        /// <summary>
        /// 测试 HashPasswordService - 加密密码
        /// </summary>
        [HttpGet("test/hash")]
        [Authorize]
        public ActionResult TestHashPassword(string password, string salt)
        {
            string hashedPassword = HashPasswordService.HashPassword(password, salt);
            return Ok(new { hashedPassword = hashedPassword });
        }

        /// <summary>
        /// 测试 HashPasswordService - 验证密码
        /// </summary>
        [HttpGet("test/validate")]
        [Authorize]
        public ActionResult TestValidate(string password, string salt, string passwordStore)
        {
            bool isValid = HashPasswordService.Validate(password, salt, passwordStore);
            return Ok(new { isValid = isValid });
        }
    }
}
