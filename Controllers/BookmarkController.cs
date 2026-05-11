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
        private readonly NetFavoriteDbContext _context;

        public BookmarkController(NetFavoriteDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bookmark>>> Get()
        {
            return Ok(await _context.Bookmark.ToListAsync());
        }

        // 保留你原有的其他接口（GET(id)/POST/PUT/DELETE），无需修改
        [HttpGet("{id}")]
        public async Task<ActionResult<Bookmark>> Get(Guid id)
        {
            var bookmark = await _context.Bookmark.FindAsync(id);
            return bookmark == null ? NotFound() : Ok(bookmark);
        }

        [HttpPost]
        public async Task<ActionResult<Bookmark>> Post([FromBody] Bookmark bookmark)
        {
            bookmark.Bookmark_Id = Guid.NewGuid();
            bookmark.Bookmark_CreateTime = DateTime.UtcNow;
            _context.Bookmark.Add(bookmark);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = bookmark.Bookmark_Id }, bookmark);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Bookmark value)
        {
            if (id != value.Bookmark_Id)
            {
                return BadRequest();
            }

            var bookmark = await _context.Bookmark.FindAsync(id);
            if (bookmark == null)
            {
                return NotFound();
            }

            bookmark.Bookmark_Title = value.Bookmark_Title;
            bookmark.Bookmark_Address = value.Bookmark_Address;

            _context.Bookmark.Update(bookmark);
            await _context.SaveChangesAsync();

            return Ok(bookmark);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var bookmark = await _context.Bookmark.FindAsync(id);
            if (bookmark == null)
            {
                return NotFound();
            }

            _context.Bookmark.Remove(bookmark);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}