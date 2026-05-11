using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NetFavorite;
using NetFavorite.Models;
using NetFavorite.Utilities;

namespace NetFavorite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoginUserController : ControllerBase
    {
        private readonly NetFavoriteDbContext _context;

        public LoginUserController(NetFavoriteDbContext context)
        {
            _context = context;
        }

        // 查询所有用户
        [HttpGet]
        [Authorize(Roles = "系统管理员")]
        public async Task<IActionResult> Get()
        {
            return Ok(
                await _context.LoginUser.Select(it =>
                new
                {
                    it.LoginUser_Id,
                    it.LoginUser_Account,
                    it.LoginUser_Role
                }).ToListAsync()
            );
        }

        // 根据ID查询单个用户
        [HttpGet("{id}")]
        [Authorize(Roles = "系统管理员")]
        public async Task<IActionResult> Get(Guid id)
        {
            var loginUser = await _context.LoginUser.FirstOrDefaultAsync(it => it.LoginUser_Id == id);
            if (loginUser == null)
            {
                return NotFound("Id不存在");
            }
            else
            {
                return Ok(
                        new
                        {
                            Id = loginUser.LoginUser_Id,
                            Account = loginUser.LoginUser_Account,
                            Role = loginUser.LoginUser_Role
                        }
                );
            }
        }

        // PUT api/<LoginUserController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Models.LoginUser value)
        {
            var user = await _context.LoginUser.FirstOrDefaultAsync(it => it.LoginUser_Id == id);
            if (user == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(value.LoginUser_Password))
            {
                return BadRequest("密码不能为空");
            }
            if (string.IsNullOrEmpty(value.LoginUser_Role))
            {
                return BadRequest("角色不能为空");
            }

            // 无论是否要更改密码，都重新生成新的随机 salt，再加密密码
            user.LoginUser_Salt = Utilities.HashPasswordService.CreateSalt();
            user.LoginUser_Password = Utilities.HashPasswordService.HashPassword(value.LoginUser_Password, user.LoginUser_Salt!);

            user.LoginUser_Role = value.LoginUser_Role;
            _context.LoginUser.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST api/<LoginUserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Models.LoginUser value)
        {
            if (string.IsNullOrEmpty(value.LoginUser_Account))
            {
                return BadRequest("用户名不能为空");
            }
            if (string.IsNullOrEmpty(value.LoginUser_Password))
            {
                return BadRequest("密码不能为空");
            }
            if (string.IsNullOrEmpty(value.LoginUser_Role))
            {
                return BadRequest("角色不能为空");
            }
            if (await _context.LoginUser.AnyAsync(it => it.LoginUser_Account == value.LoginUser_Account))
            {
                return BadRequest("用户名重名");
            }

            // 生成随机 salt，再加密密码
            value.LoginUser_Salt = Utilities.HashPasswordService.CreateSalt();
            value.LoginUser_Password = Utilities.HashPasswordService.HashPassword(value.LoginUser_Password, value.LoginUser_Salt!);

            value.LoginUser_Id = Guid.NewGuid();
            _context.LoginUser.Add(value);
            await _context.SaveChangesAsync();
            
            return Ok(new
            {
                Id = value.LoginUser_Id,
                Account = value.LoginUser_Account,
                Role = value.LoginUser_Role
            });
        }

        // 删除用户
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoginUser(Guid id)
        {
            var loginUser = await _context.LoginUser.FindAsync(id);
            if (loginUser == null)
            {
                return NotFound();
            }

            _context.LoginUser.Remove(loginUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}