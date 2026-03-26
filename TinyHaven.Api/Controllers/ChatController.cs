using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TinyHaven.Api.Data;

namespace TinyHaven.Api.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly AppDbContext _db;

        public ChatController(AppDbContext db)
        {
            _db = db;
        }
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] int take)
        {
            take = take <= 0 ? 50 : Math.Clamp(take, 1, 100);

            var messages = await _db.ChatMessages
                .AsNoTracking()
                .Include(x => x.Sender)
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .Select(x => new
                {
                    x.Id,
                    x.SenderId,
                    SenderName = x.Sender != null ? x.Sender.DisplayName : "Ẩn danh",
                    x.Message,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(messages.OrderBy(x => x.CreatedAt));

        }
    }
}
