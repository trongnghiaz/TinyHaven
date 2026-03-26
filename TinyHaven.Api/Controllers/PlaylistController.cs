using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TinyHaven.Api.Data;
using TinyHaven.Api.Models;
using static TinyHaven.Api.Dtos.PlaylistDtos;

namespace TinyHaven.Api.Controllers
{
    [Route("api/playlist")]
    [ApiController]
    [Authorize]
    public class PlaylistController : BaseController
    {
        private readonly AppDbContext _db;

        public PlaylistController(AppDbContext db)
        {
            _db = db;
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.Playlists
               .AsNoTracking()
               .Include(x => x.Tracks)
               .OrderByDescending(x => x.CreatedAt)
               .Select(x => new
               {
                   x.Id,
                   x.Title,
                   x.CreatedById,
                   x.CreatedAt,
                   Tracks = x.Tracks
                       .OrderBy(t => t.SortOrder)
                       .Select(t => new
                       {
                           t.Id,
                           t.Title,
                           t.Artist,
                           t.SourceUrl,
                           t.SortOrder,
                           t.DurationSeconds
                       })
               })
               .ToListAsync();

            return Ok(items);

        }
        [HttpPost("post")]
        public async Task<IActionResult> Create([FromBody] CreatePlaylistRequest request)
        {
            var userId = GetUserId(User);

            var entity = new Playlist
            {
                Title = request.Title,
                CreatedById = userId
            };

            _db.Playlists.Add(entity);
            await _db.SaveChangesAsync();

            return Ok(entity);
        }
        [HttpPost("add-track/{playlistId:guid}/tracks")]
        public async Task<IActionResult> AddTrack(Guid playlistId, [FromBody] AddTrackRequest request)
        {
            var playlist = await _db.Playlists.FirstOrDefaultAsync(x => x.Id == playlistId);
            if (playlist is null) return NotFound();

            var track = new PlaylistTrack
            {
                PlaylistId = playlistId,
                Title = request.Title,
                Artist = request.Artist,
                SourceUrl = request.SourceUrl,
                SortOrder = request.SortOrder,
                DurationSeconds = request.DurationSeconds
            };

            _db.PlaylistTracks.Add(track);
            await _db.SaveChangesAsync();

            return Ok(track);

        }
        [HttpPut("update-track/{playlistId:guid}/tracks/{trackId:guid}")]
        public async Task<IActionResult> UpdateTrack(Guid playlistId, Guid trackId, [FromBody] UpdateTrackRequest request)
        {
            var track = await _db.PlaylistTracks.FirstOrDefaultAsync(x => x.Id == trackId && x.PlaylistId == playlistId);
            if (track is null) return NotFound();
            track.Title = request.Title;
            track.Artist = request.Artist;
            track.SourceUrl = request.SourceUrl;
            track.SortOrder = request.SortOrder;
            track.DurationSeconds = request.DurationSeconds;
            await _db.SaveChangesAsync();
            return Ok(track);

        }
        [HttpDelete("delete-track/{playlistId:guid}/tracks/{trackId:guid}")]
        public async Task<IActionResult> DeleteTrack(Guid playlistId, Guid trackId)
        {
            var track = await _db.PlaylistTracks.FirstOrDefaultAsync(x => x.Id == trackId && x.PlaylistId == playlistId);
            if (track is null) return NotFound();
            _db.PlaylistTracks.Remove(track);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Track removed" });

        }
    }
}
