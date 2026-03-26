using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TinyHaven.Api.Data;
using TinyHaven.Api.Models;
using static TinyHaven.Api.Dtos.DiaryDtos;

namespace TinyHaven.Api.Controllers
{
    [Route("api/diary")]
    [ApiController]
    [Authorize]
    public class DiaryController : BaseController
    {
        private readonly AppDbContext _db;

        public DiaryController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.DiaryEntries
                .AsNoTracking()
                .Include(x => x.CreatedBy)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Content,
                    x.Mood,
                    x.CreatedAt,
                    x.UpdatedAt,
                    x.CreatedById,
                    Author = x.CreatedBy != null ? x.CreatedBy.DisplayName : "Ẩn danh"
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDiaryRequest request)
        {
            var userId = GetUserId(User);

            var entity = new DiaryEntry
            {
                Title = request.Title,
                Content = request.Content,
                Mood = request.Mood,
                CreatedById = userId
            };

            _db.DiaryEntries.Add(entity);
            await _db.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDiaryRequest request)
        {
            var userId = GetUserId(User);
            var entity = await _db.DiaryEntries.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null) return NotFound();
            if (entity.CreatedById != userId) return Forbid();

            entity.Title = request.Title;
            entity.Content = request.Content;
            entity.Mood = request.Mood;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId(User);
            var entity = await _db.DiaryEntries.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null) return NotFound();
            if (entity.CreatedById != userId) return Forbid();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
