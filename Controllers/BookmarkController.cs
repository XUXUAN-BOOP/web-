using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetFavorite.Models;

namespace NetFavorite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookmarkController : ControllerBase
    {
        private readonly NetFavoriteDbContext _db;
        private readonly Utilities.ITokenService _tokenService;
        private readonly Models.LoginUser loginUser;

        public BookmarkController(NetFavoriteDbContext db, Utilities.ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
            loginUser = _tokenService.ReadToken();
        }

        [HttpGet]
        public IEnumerable<Models.Bookmark> Get()
        {
            return _db.Bookmark
                .Where(it => it.Bookmark_LoginUserId == loginUser.LoginUser_Id)
                .ToList();
        }

        // GET api/<BookmarkController>/5
        // 按照主键，查询单个实例并返回
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var bookmark = _db.Bookmark.FirstOrDefault(it => it.Bookmark_Id == id);
            if (bookmark == null) return NotFound();

            if (bookmark.Bookmark_LoginUserId != loginUser.LoginUser_Id) return Forbid();
            return Ok(bookmark);
        }

        // POST api/<BookmarkController>
        // 创建
        [HttpPost]
        public IActionResult Post([FromBody] Models.Bookmark bookmark)
        {
            if (string.IsNullOrEmpty(bookmark.Bookmark_Address))
            {
                return BadRequest();
            }

            bookmark.Bookmark_Id = Guid.NewGuid();
            bookmark.Bookmark_CreateTime = DateTime.Now;
            bookmark.Bookmark_LoginUserId = loginUser.LoginUser_Id;

            _db.Bookmark.Add(bookmark);
            _db.SaveChanges();
            return Ok(bookmark);
        }

        // PUT api/<BookmarkController>/5
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] Models.Bookmark value)
        {
            var bookmark = _db.Bookmark.FirstOrDefault(it => it.Bookmark_Id == id);
            if (bookmark == null) return NotFound();

            if (bookmark.Bookmark_LoginUserId != loginUser.LoginUser_Id) return Forbid();

            bookmark.Bookmark_Title = value.Bookmark_Title;
            bookmark.Bookmark_Address = value.Bookmark_Address;
            _db.Bookmark.Update(bookmark);
            _db.SaveChanges();
            return Ok();
        }

        // DELETE api/<BookmarkController>/5
        //删除
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var bookmark = _db.Bookmark.FirstOrDefault(it => it.Bookmark_Id == id);
            if (bookmark == null) return NotFound();

            if (bookmark.Bookmark_LoginUserId != loginUser.LoginUser_Id) return Forbid();

            _db.Bookmark.Remove(bookmark);
            _db.SaveChanges();
            return Ok();
        }
    }
}