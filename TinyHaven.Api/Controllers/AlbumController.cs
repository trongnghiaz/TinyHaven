using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TinyHaven.Api.Data;
using TinyHaven.Api.Models;

namespace TinyHaven.Api.Controllers
{
    [Route("api/album")]
    [ApiController]
    [Authorize]
    public class AlbumController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly string _uploadRoot;

        public AlbumController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _uploadRoot = config["UploadPath"] ?? Path.Combine("wwwroot", "uploads");

            if (!Directory.Exists(_uploadRoot))
                Directory.CreateDirectory(_uploadRoot);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.AlbumPhotos
               .AsNoTracking()
               .Include(x => x.UploadedBy)
               .OrderByDescending(x => x.CreatedAt)
               .Select(x => new
               {
                   x.Id,
                   x.Url,
                   x.Caption,
                   x.ContentType,
                   x.FileSize,
                   x.CreatedAt,
                   x.UploadedById,
                   UploadedBy = x.UploadedBy != null ? x.UploadedBy.DisplayName : null
               })
               .ToListAsync();

            return Ok(items);

        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(
            [FromForm] IFormFile file,
            [FromForm] string? caption)
        {
            var userId = GetUserId(User);

            if (file is null || file.Length == 0)
                return BadRequest(new { message = "File rỗng" });

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest(new { message = "Chỉ chấp nhận file ảnh" });

            const long maxFileSize = 10 * 1024 * 1024;
            if (file.Length > maxFileSize)
                return BadRequest(new { message = "File vượt quá 10MB" });

            var extension = Path.GetExtension(Path.GetFileName(file.FileName)).ToLowerInvariant();
            var safeFileName = $"{Guid.NewGuid()}{extension}";
            var savePath = Path.Combine(_uploadRoot, safeFileName);

            await using (var stream = System.IO.File.Create(savePath))
            {
                await file.CopyToAsync(stream);
            }

            var entity = new AlbumPhoto
            {
                FileName = safeFileName,
                OriginalFileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType,
                FileSize = file.Length,
                Url = $"/uploads/{safeFileName}",
                Caption = caption,
                UploadedById = userId
            };

            _db.AlbumPhotos.Add(entity);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                entity.Id,
                entity.Url,
                entity.Caption,
                entity.ContentType,
                entity.FileSize,
                entity.CreatedAt
            });

        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id) 
        {
            var userId = GetUserId(User);
            var entity = await _db.AlbumPhotos.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null) return NotFound();
            if (entity.UploadedById != userId) return Forbid();

            var physicalPath = Path.Combine(_uploadRoot, entity.FileName);
            if (System.IO.File.Exists(physicalPath))
                System.IO.File.Delete(physicalPath);

            _db.AlbumPhotos.Remove(entity);
            await _db.SaveChangesAsync();

            return NoContent();

        }

        
    }
}
