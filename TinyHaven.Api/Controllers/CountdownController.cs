using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TinyHaven.Api.Data;
using static TinyHaven.Api.Dtos.CountdownDtos;

namespace TinyHaven.Api.Controllers
{
    [Route("api/count-down")]
    [ApiController]
    [Authorize]
    public class CountdownController : BaseController
    {
        private readonly AppDbContext _db;

        public CountdownController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetCurrent()
        {
            var item = await _db.CountdownEvents
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Description,
                    x.TargetDate,
                    x.CreatedAt,
                    x.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return item is null ? NotFound() : Ok(item);
        }

        [HttpPut("put")]
        public async Task<IActionResult> Upsert([FromBody] CountdownUpsertRequest request)
        {
            var userId = GetUserId(User);

            var item = await _db.CountdownEvents
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (item == null)
            {
                item = new Models.CountdownEvent
                {
                    Title = request.Title,
                    Description = request.Description,
                    TargetDate = request.TargetDate,
                    CreatedById = userId,
                    IsActive = true
                };
                _db.CountdownEvents.Add(item);
            }
            else
            {
                item.Title = request.Title;
                item.Description = request.Description;
                item.TargetDate = request.TargetDate;
                item.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await _db.SaveChangesAsync();

            return Ok(new { item.Id, item.Title, item.Description, item.TargetDate, item.CreatedAt, item.UpdatedAt });
        }
    }
}
